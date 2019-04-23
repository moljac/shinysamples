using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny.IO;
using Shiny.Net.Http;


namespace Samples.HttpTransfers
{
    public class LsDownload : ReactiveObject
    {
        public string CurrentId { get; set; }
        public string Path { get; set; }
        [Reactive] public string Status { get; set; }
        [Reactive] public double PercentComplete { get; set; }
        public ICommand Download { get; set; }
    }


    public class LsViewModel : ViewModel
    {
        readonly IHttpTransferManager transfers;
        readonly IFileSystem fileSystem;
        readonly HttpClient httpClient;


        public LsViewModel(IHttpTransferManager transfers, IFileSystem fileSystem)
        {
            this.transfers = transfers;
            this.fileSystem = fileSystem;

            this.httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.lifespeak.rocks/")
            };
            this.httpClient.DefaultRequestHeaders.Add("X-LS-Auth-Token", "E1675B57-43E4-47B8-B3DC-AAE554810994");

            this.transfers
                .WhenUpdated()
                .SubOnMainThread(
                    x =>
                    {
                        var dl = this.Downloads.FirstOrDefault(y => y.CurrentId == x.Identifier);
                        if (dl != null)
                        {
                            dl.PercentComplete = x.PercentComplete * 100;
                            dl.Status = x.Status == HttpTransferState.Error
                                ? "[ERROR] " + x.Exception
                                : x.Status.ToString();
                        }
                    },
                    ex => Console.WriteLine("[BOOM] " + ex)
                )
                .DisposeWith(this.DestroyWith);

            this.Downloads = new List<LsDownload>
            {
                this.Create("Baker/Baker_1.mp4", "Baker_1.mp4"),
                this.Create("Baker/Baker_2.mp4", "Baker_2.mp4"),
                this.Create("Baker/Baker_3.mp4", "Baker_3.mp4"),
                this.Create("Baker/Baker_4.mp4", "Baker_4.mp4"),
                this.Create("Baker/Baker_5.mp4", "Baker_5.mp4"),
                this.Create("Baker/Baker_6.mp4", "Baker_6.mp4"),
                this.Create("AmidorEatingforOptimalHealth20179Nov/Amidor_Toby_EatingOptimalHealth_1.mp4", "Amidor_Toby_EatingOptimalHealth_1.mp4"),
                this.Create("AmidorEatingforOptimalHealth20179Nov/Amidor_Toby_EatingOptimalHealth_2.mp4", "Amidor_Toby_EatingOptimalHealth_2.mp4"),
                this.Create("AmidorEatingforOptimalHealth20179Nov/Amidor_Toby_EatingOptimalHealth_3.mp4", "Amidor_Toby_EatingOptimalHealth_3.mp4"),
                this.Create("AmidorEatingforOptimalHealth20179Nov/Amidor_Toby_EatingOptimalHealth_4.mp4", "Amidor_Toby_EatingOptimalHealth_4.mp4"),
                this.Create("AmidorEatingforOptimalHealth20179Nov/Amidor_Toby_EatingOptimalHealth_5.mp4", "Amidor_Toby_EatingOptimalHealth_5.mp4"),
                this.Create("AmidorEatingforOptimalHealth20179Nov/Amidor_Toby_EatingOptimalHealth_6.mp4", "Amidor_Toby_EatingOptimalHealth_6.mp4"),
                this.Create("SniderAdlerTheCompleteGuidetoCannabis20197Feb/SniderAdler_Melissa_Cannabis_1.mp4", "SniderAdler_Melissa_Cannabis_1.mp4"),
                this.Create("SniderAdlerTheCompleteGuidetoCannabis20197Feb/SniderAdler_Melissa_Cannabis_2.mp4", "SniderAdler_Melissa_Cannabis_2.mp4"),
                this.Create("SniderAdlerTheCompleteGuidetoCannabis20197Feb/SniderAdler_Melissa_Cannabis_3.mp4", "SniderAdler_Melissa_Cannabis_3.mp4"),
                this.Create("SniderAdlerTheCompleteGuidetoCannabis20197Feb/SniderAdler_Melissa_Cannabis_4.mp4", "SniderAdler_Melissa_Cannabis_4.mp4"),
                this.Create("SniderAdlerTheCompleteGuidetoCannabis20197Feb/SniderAdler_Melissa_Cannabis_5.mp4", "SniderAdler_Melissa_Cannabis_5.mp4"),
                this.Create("SniderAdlerTheCompleteGuidetoCannabis20197Feb/SniderAdler_Melissa_Cannabis_6.mp4", "SniderAdler_Melissa_Cannabis_6.mp4")
            };
        }


        public ICommand Download { get; }
        public IList<LsDownload> Downloads { get; }


        LsDownload Create(string path, string fileName)
        {
            var store = Path.Combine(this.fileSystem.AppData.FullName, fileName);
            var dl = new LsDownload
            {
                Path = store,
                Status = "None",
                PercentComplete = 0
            };
            dl.Download = ReactiveCommand.CreateFromTask(async () =>
            {
                var uri = await httpClient.GetStringAsync($"/api/assetDetails/getawsurlforclip?key=VideosLowRes/{path}");
                uri = uri.Trim('"');
                Console.WriteLine(uri);
                var task = await transfers.Enqueue(new HttpTransferRequest(uri, dl.Path));
                dl.CurrentId = task.Identifier;
            });
            return dl;
        }
    }
}
