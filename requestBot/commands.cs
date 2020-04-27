using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.General;
using System.IO;
using Newtonsoft.Json;

namespace requestBot
{

    class commands
    {


        //TMDbClient client = new TMDbClient();
        //String ombiAPI = "b3e5109461a747f4a71d48f3c299b38c";
        //get the api key from the public struct configjson, can't read it for some reason.
        //maybe figure out what a struct is in the first place


        [Command("ping")] //pings the bot
        [Description("Example ping command")]
        [Aliases("pong")] 
        public async Task Ping(CommandContext ctx) // this command takes no arguments
        {
            await ctx.TriggerTypingAsync(); //let the user know that were working on it.
            var emoji = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");
            await ctx.RespondAsync($"{emoji} Pong! Ping: {ctx.Client.Ping}ms");
        }


        [Command("request"), Description("Requests a movie."), Aliases("addmovie")]
        public async Task requestMovie(CommandContext ctx, [RemainingText, Description("The movie you would like to add")] string request)
        {
            // let's trigger a typing indicator to let
            // users know we're working
            await ctx.TriggerTypingAsync();

            if (String.IsNullOrEmpty(request)) //checks to see if the user has put in a request query
            {
                await ctx.RespondAsync("Request can not be blank! The correct format is .request movie ");
            }
            else
            {
                try
                {
                    var cameraEmoji = DiscordEmoji.FromName(ctx.Client, ":movie_camera:"); //uses the movie camera emoji, this can be changed to any emoji.
                    var errorEmoji = DiscordEmoji.FromName(ctx.Client, ":exclamation:");
                    var checkEmoji = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");
                    int count = 1;
                    int maxCycles = 3; //the max number of results to cycle through
                    var interactivity = ctx.Client.GetInteractivityModule();
                    //get the ombi and the tmdb api key
                    var json = "";
                    using (var fs = File.OpenRead("config.json"))
                    using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                        json = await sr.ReadToEndAsync();
                    var cfgjson = JsonConvert.DeserializeObject<ConfigJson>(json);


                    TMDbClient client = new TMDbClient(cfgjson.tmdbApiKey);

                    SearchContainer<SearchMovie> results = searchMovie.getResultsAsync(request, client).Result; //gets movie results bases on query 
                                                                                                                //cycles through three movie results, asking the user each time if it's the correct result.
                    foreach (SearchMovie result in results.Results.Take(maxCycles))
                    {
                        // Print out each hit                         
                        await ctx.TriggerTypingAsync();
                        await ctx.RespondAsync(result.Id + ": " + result.Title +
                        "\n Original Title: " + result.OriginalTitle +
                        "\n Release date  : " + result.ReleaseDate +
                        "\n Popularity    : " + result.Popularity +
                        "\n Vote Average  : " + result.VoteAverage +
                        "\n Vote Count    : " + result.VoteCount +

                        "\n" + "http://image.tmdb.org/t/p/w185//" + result.PosterPath);

                        await ctx.TriggerTypingAsync();
                        await ctx.RespondAsync("Is this the movie you would like to request?");
                        var msg = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id, TimeSpan.FromMinutes(1));

                        if (msg != null)
                        {
                            String Message = msg.Message.Content.ToLower();
                            if (Message == "yes")
                            {
                                await ctx.RespondAsync("Okay, give me a second.");
                                await ctx.TriggerTypingAsync();

                                String requestResult = sendRequest.requestMovie(result.Id, cfgjson.ombiApiKey);
                                responseHeaders responseHeaders = JsonConvert.DeserializeObject<responseHeaders>(requestResult); //deserializes the json response header (requestResult), passes it to the responseHeader object.
                                if (responseHeaders.isError == true)
                                {
                                    await ctx.TriggerTypingAsync();
                                    await ctx.RespondAsync(responseHeaders.errorMessage + " " + errorEmoji);
                                    break;
                                }
                                else if (responseHeaders.result == true)
                                {
                                    await ctx.TriggerTypingAsync();
                                    await ctx.RespondAsync(responseHeaders.message + " " + cameraEmoji + " " + checkEmoji);
                                    break;
                                }
                                else
                                {
                                    await ctx.RespondAsync("Something went wrong, try again.");
                                    break;
                                }
                            }
                            else if (Message == "no" && count < maxCycles)
                            {
                                if (count + 1 > results.Results.Count)
                                {
                                    await ctx.TriggerTypingAsync();
                                    await ctx.RespondAsync("No more results found. Make sure that you've typed the name correctly.");
                                    break;
                                }

                                await ctx.TriggerTypingAsync();
                                await ctx.RespondAsync("Okay, how about this?");
                                count++;
                            }
                            else
                            {
                                await ctx.TriggerTypingAsync();
                                await ctx.RespondAsync("Unfortunately, your request could not be found. Please make sure that you've typed the name correctly.");
                                break;
                            }
                        }
                        else
                        {
                            await ctx.TriggerTypingAsync();
                            await ctx.RespondAsync("No response found, please try again.");
                            break;
                        }

                    }


                }
                catch (Exception ex)
                {
                    // something failed, let the invoker now
                    var emoji = DiscordEmoji.FromName(ctx.Client, ":-1:");
                    await ctx.RespondAsync("Something went wrong. Please try again." + emoji);
                    Console.WriteLine("Exception thrown: " + ex.Message);
                }
            }
        }


    }

    public class responseHeaders
    {
        public bool result { get; set; }
        public string message { get; set; }
        public bool isError { get; set; }
        public string errorMessage { get; set; }
        public int requestId { get; set; }
    }

}
