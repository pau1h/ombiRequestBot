# ombiRequestBot
A Discord request bot for Ombi. Uses the DSharpPlus API for the bot. As of right now, only works for movies.

Add config.json to the debug directory with this information: 

```
{
  "token": "Your discord bot token",
  "prefix": ".",
  "tmdbApiKey": "Your api key from https://www.themoviedb.org/ , used to fetch movies",
  "ombiApiKey": "Your Ombi Api key, which can be found in ",
  "ombiUrl": "Your ombi URL, in this format, http://192.168.1.219:5000"
}
```
