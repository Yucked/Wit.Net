<!--- Artificial Intelligence by Andrew Forrester from the Noun Project --->
<p align="center">
    <img src="https://media.discordapp.net/attachments/253326058630283264/458260513965604875/Untitled-2.png" width="15%" />
    <h2 align="center">WitSharp</h2>
    <p align="center">
        <a href="https://ci.appveyor.com/project/Yucked/witsharp"><img src="https://ci.appveyor.com/api/projects/status/kbly9aehvuyp9idf/branch/master"/></a>
  <a href="https://www.buymeacoffee.com/Yucked" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/black_img.png" width="130px"></a>
    <a href="https://www.nuget.org/packages/WitSharp/"><img src="https://img.shields.io/nuget/v/WitSharp.svg?longCache=true&style=flat-square" ></a>
    </p>        

> Usage

**Creating new application:**
```cs
var Client = new WitClient("MY-APPLICATION-TOKEN");
var Create = await Client.Application.CreateAsync("WitSharp", Language.EN, true).ConfigureAwait(false);
Console.WriteLine(Create.AppId);
```

**Getting sentence meaning:**
```cs
var Meaning = Client.Training.SentenceMeaningAsync(new SentenceModel {
    Message = "How many people between monday and friday>",
    MaxTraits = 4
    }).ConfigureAwait(false);
Console.WriteLine(Meaning.Entities.Intents.FirstOrDefault().Value);
```

> ToDo

- [ ] Audio Support
- [ ] Languages
