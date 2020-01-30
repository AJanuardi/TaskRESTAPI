using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using System.Text.Json;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace ToDo
{
    class Program
    {

        static async Task<int> Main(string[] args)
        {
            var rootApp = new CommandLineApplication()
            {
                Name = "Aplikasi ToDoList",
                Description = "Ini digunakan untuk mendata kegiatan harian",
                ShortVersionGetter = () => "1.0.0"
            };
            rootApp.Command("todo",app => 
            {
                app.Description = "todo app";
                // var url = app.Argument("text","masukkan text");
                var list = app.Option("--list", "cek list", CommandOptionType.SingleOrNoValue);
                var clear = app.Option("--clear", "cek list", CommandOptionType.SingleOrNoValue);
                var add = app.Option("--add","cek list", CommandOptionType.SingleOrNoValue);
                var done = app.Option("--done", "cek list", CommandOptionType.SingleOrNoValue);
                var delete = app.Option("--delete", "cek list", CommandOptionType.SingleOrNoValue);
                var update = app.Option("--update", "cek list", CommandOptionType.MultipleValue);

                app.OnExecuteAsync(async cancellationToken =>
                {
                    HttpClientHandler clientHandler = new HttpClientHandler();
                    clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                    HttpClient client = new HttpClient(clientHandler);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get,"https://localhost:5001/todo");
                    if (list.HasValue())
                    {
                        HttpResponseMessage response = await client.SendAsync(request);
                        var data = await response.Content.ReadAsStringAsync();
                        var ObjekList = JsonConvert.DeserializeObject<List<todo>>(data);
                        Console.WriteLine("List Kegiatan: ");
                            foreach(var x in ObjekList)
                            {
                                Console.WriteLine(x.kegiatan);
                            }
                    }
                    if (clear.HasValue())
                    {
                        HttpResponseMessage response1 = await client.SendAsync(request);
                        var data = await response1.Content.ReadAsStringAsync();
                        var ObjekList = JsonConvert.DeserializeObject<List<todo>>(data);
                        var sure = Prompt.GetYesNo("Yakin", false);

                        if (sure)
                        {
                            foreach (var x in ObjekList)
                            {
                                await client.DeleteAsync($"https://localhost:5001/todo/{x.id}");
                            }
                        }
                    }
                    if (delete.HasValue())
                    {
                        HttpResponseMessage response1 = await client.SendAsync(request);
                        var data = await response1.Content.ReadAsStringAsync();
                        var ObjekList = JsonConvert.DeserializeObject<List<todo>>(data);
                        var del = Convert.ToInt32(delete.Value());    
                            foreach (var x in ObjekList)
                            {
                                if (x.id == del)
                                {
                                await client.DeleteAsync($"http://localhost:3000/ToDo/{x.id}");
                                }
                            }
                    }
                    if (update.HasValue())
                    {
                        HttpResponseMessage response1 = await client.SendAsync(request);
                        var data = await response1.Content.ReadAsStringAsync();
                        var ObjekList = JsonConvert.DeserializeObject<List<todo>>(data);
                        Console.WriteLine("masukkan todo list");
                        var input = Console.ReadLine();
                        var obj = new todo()
                        {
                            kegiatan = input
                        };
                        var toJson = JsonConvert.SerializeObject(obj);
                        var cnt = new StringContent(toJson, Encoding.UTF8, "application/json");
                        var id = Convert.ToInt32(update.Value());
                        foreach (var x in ObjekList)
                        {
                            if (x.id== id)
                            {
                                await client.PatchAsync($"https://localhost:5001/todo/{x.id}",cnt);
                            }
                        }
                    }
                    if (add.HasValue())
                    {
                        var tambah = new todo()
                        {
                            kegiatan = add.Value()
                        };
                        var Objek = JsonConvert.SerializeObject(tambah);
                        var httpContent = new StringContent(Objek, Encoding.UTF8, "application/json");
                        var data1 = await client.PostAsync("https://localhost:5001/todo", httpContent);
                    }
                    if (done.HasValue())
                    {
                        var tambah = new todo()
                        {
                            kegiatan = add.Value()
                        };
                        var obj = JsonConvert.SerializeObject(tambah);
                        var Objek = new StringContent(obj, Encoding.UTF8, "application/json");
                        var data1 = await client.PatchAsync($"https://localhost:5001/todo/done/{done.Value()}", Objek);
                    }
                
                });
            });
           return rootApp.Execute(args);
        }
    }
    public class todo
    {
        public int id {get; set;}
        public string kegiatan {get;set;}
        public bool selesai {get; set;}
    }
}
