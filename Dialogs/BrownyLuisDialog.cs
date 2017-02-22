using System;
using System.Threading.Tasks;
using BrownyBot.Internal;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace BrownyBot.Dialogs
{
  [Serializable]
  [LuisModel("efe82f28-1886-48fe-825b-2e5fd57a0706", "6ac3b4cf3c77414982be4fb8ed4bbd43")]
  class BrownyLuisDialog : LuisDialog<object>
  {
    [LuisIntent("None")]
    [LuisIntent("")]
    public async Task ProcessNone(IDialogContext context, LuisResult result)
    {
      await context.PostAsync("I'm sorry. I didn't understand you.");
      context.Wait(MessageReceived);
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

    [LuisIntent("Hello")]
    public async Task ProcessHello(IDialogContext context, LuisResult result)
    {
      var messages = new[]
      {
                "Hello!",
                "Nice to meet you!",
                "Hi! What can I help you with?",
                "I'm here to help you!"
      };

      var message = messages[(new Random()).Next(messages.Length - 1)];
      await context.PostAsync(message, "en-US");

      context.Wait(MessageReceived);
    }

    [LuisIntent("Thanks")]
    public async Task ProcessThanks(IDialogContext context, LuisResult result)
    {
      var messages = new[]
      {
                "Never mind",
                "You are welcome!",
                "Happy to be useful"
      };

      var message = messages[(new Random()).Next(messages.Length - 1)];
      await context.PostAsync(message, "en-US");

      context.Wait(MessageReceived);
    }

  }

}