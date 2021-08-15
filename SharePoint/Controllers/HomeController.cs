using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SharePoint.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using Microsoft.SharePoint.Client;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace SharePoint.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        Uri site = new Uri("https://charlie619.sharepoint.com/sites/sampleSite");
        string user = "charlie@charlie619.onmicrosoft.com";
        string rawPassword = "Apple@3579";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetSongs([DataSourceRequest] DataSourceRequest request)
        {
            var listItems = new List<Songs>();

            SecureString password = new SecureString();
            foreach (char c in rawPassword) password.AppendChar(c);

            using (ConnectionSetting authenticationManager = new ConnectionSetting())
            using (var context = authenticationManager.GetContext(site, user, password))
            {

                List announcementsList = context.Web.Lists.GetByTitle("Songs");

                CamlQuery query = CamlQuery.CreateAllItemsQuery(100);
                ListItemCollection items = announcementsList.GetItems(query);

                context.Load(items);
                context.ExecuteQuery();
                foreach (ListItem row in items)
                {
                    var obj = new Songs()
                    {
                        Id = (int)row["ID"],
                        Title = (string)row["Title"],
                        Author = (string)row["Author0"],
                        ReleaseDate = (double)row["ReleaseDate"]
                    };

                    listItems.Add(obj);
                }
                var dsResult = listItems.ToDataSourceResult(request);
                return Json(dsResult);
            }
        }

        [HttpPost]
        public IActionResult CreateSong([DataSourceRequest] DataSourceRequest request, Songs song)
        {
            if (ModelState.IsValid)
            {
                SecureString password = new SecureString();
                foreach (char c in rawPassword) password.AppendChar(c);

                using (ConnectionSetting authenticationManager = new ConnectionSetting())
                using (var context = authenticationManager.GetContext(site, user, password))
                {
                    var oList = context.Web.Lists.GetByTitle("Songs");
                    ListItemCreationInformation data = new ListItemCreationInformation();
                    ListItem oListItem = oList.AddItem(data);

                    oListItem["Title"] = song.Title;
                    oListItem["Author0"] = song.Author;
                    oListItem["ReleaseDate"] = song.ReleaseDate;

                    oListItem.Update();
                    context.ExecuteQuery();

                }
            }

            return Json(new[] { song }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public IActionResult UpdateSong([DataSourceRequest] DataSourceRequest request, Songs song)
        {
            if (ModelState.IsValid)
            {
                SecureString password = new SecureString();
                foreach (char c in rawPassword) password.AppendChar(c);

                using (ConnectionSetting authenticationManager = new ConnectionSetting())
                using (var context = authenticationManager.GetContext(site, user, password))
                {
                    var oList = context.Web.Lists.GetByTitle("Songs");
                    ListItem listItem = oList.GetItemById(song.Id);

                    listItem["Title"] = song.Title;
                    listItem["Author0"] = song.Author;
                    listItem["ReleaseDate"] = song.ReleaseDate;
                    listItem.Update();

                    context.ExecuteQuery();
                }
            }
            return Json(new[] { song }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public IActionResult DeleteSong([DataSourceRequest] DataSourceRequest request, Songs song)
        {

            SecureString password = new SecureString();
            foreach (char c in rawPassword) password.AppendChar(c);

            using (ConnectionSetting authenticationManager = new ConnectionSetting())
            using (var context = authenticationManager.GetContext(site, user, password))
            {
                var oList = context.Web.Lists.GetByTitle("Songs");
                ListItem listItem = oList.GetItemById(song.Id);

                listItem.DeleteObject();
                context.ExecuteQuery();
            }

            return Json(new[] { song }.ToDataSourceResult(request));
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
