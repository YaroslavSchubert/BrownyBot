using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BrownyBot.Internal;
using BrownyBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace BrownyBot.Dialogs
{
  [Serializable]
  [LuisModel("efe82f28-1886-48fe-825b-2e5fd57a0706", "6ac3b4cf3c77414982be4fb8ed4bbd43")]
  class BrownyLuisDialog : LuisDialog<object>
  {
    [LuisIntent("None")]
    [LuisIntent("")]
    public async Task ProcessNone(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result)
    {
      var cts = new CancellationTokenSource();
      await context.Forward(new GreetingsDialog(), GreetingDialogDone, await message, cts.Token);
    }

    [LuisIntent("AboutMe")]
    public async Task ProcessAboutMe(IDialogContext context, LuisResult result)
    {
      await context.PostAsync(Responses.AboutBrowny);
      context.Wait(MessageReceived);
    }

    [LuisIntent("AboutAuthor")]
    public async Task ProcessAboutAuthor(IDialogContext context, LuisResult result)
    {
      await context.PostAsync(Responses.AboutAuthor);
      context.Wait(MessageReceived);
    }

    [LuisIntent("Help")]
    public async Task ProcessHelp(IDialogContext context, LuisResult result)
    {
      await context.PostAsync(Responses.Help);
      context.Wait(MessageReceived);
    }

    [LuisIntent("Weather")]
    public async Task ProcessWeather(IDialogContext context, LuisResult result)
    {
      await context.PostAsync("Weather");
      context.Wait(MessageReceived);
    }

    [LuisIntent("Caption")]
    public async Task ProcessImageCaption(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result)
    {
      var cts = new CancellationTokenSource();
      await context.Forward(new CaptionDialog(), ImageCaptionDialogDone, await message, cts.Token);
    }

    private async Task ImageCaptionDialogDone(IDialogContext context, IAwaitable<object> result)
    {
      await result;
      await context.PostAsync("Caption mode ended");
      context.Wait(MessageReceived);
    }

    private async Task GreetingDialogDone(IDialogContext context, IAwaitable<bool> result)
    {
      var success = await result;
      if (!success)
        await context.PostAsync("I'm sorry. I didn't understand you.");

      context.Wait(MessageReceived);
    }
  }
}