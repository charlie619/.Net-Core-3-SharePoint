using Microsoft.AspNetCore.Mvc;
using Microsoft.SharePoint.Client;
using SharePoint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SharePoint.Controllers
{
    [Route("api/Sharepoint")]
    [ApiController]
    public class SharepointController : ControllerBase
    {
        public SharepointController(IConfiguration config)
        {
            _config = config;
            rawPassword = _config.GetValue<string>("password");
            user = _config.GetValue<string>("user");
            site = new Uri(_config.GetValue<string>("url"));
        }
        public IConfiguration _config { get; }
        public string user { get; set; }
        public string rawPassword { get; set; }
        public Uri site { get; set; }


        // GET: api/<SharepointController>
        [HttpGet]
        public IActionResult GetSongs()
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
                //var json = JsonConvert.SerializeObject(listItems);
                return Ok(listItems);
            }
        }        

        // POST api/<SharepointController>
        [HttpPost]
        public async Task<IActionResult> CreateSong(Songs song)
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
                    await context.ExecuteQueryAsync();
                }
            }

            return Ok("Song has been created successfully!!!");
        }

        // PUT api/<SharepointController>/5
        [HttpPut]
        public async Task<IActionResult> UpdateSong(Songs song)
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

                   await context.ExecuteQueryAsync();
                }
            }
            return Ok("Song has been Updated!!!");
        }

        // DELETE api/<SharepointController>/5
        [HttpDelete]
        public async Task<IActionResult> DeleteSong(Songs song)
        {
            SecureString password = new SecureString();
            foreach (char c in rawPassword) password.AppendChar(c);

            using (ConnectionSetting authenticationManager = new ConnectionSetting())
            using (var context = authenticationManager.GetContext(site, user, password))
            {
                var oList = context.Web.Lists.GetByTitle("Songs");
                ListItem listItem = oList.GetItemById(song.Id);

                listItem.DeleteObject();
                await context.ExecuteQueryAsync();
            }

            return Ok("Song has been deleted!!!");
        }
    }
}
