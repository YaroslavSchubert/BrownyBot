﻿using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using BestMatchDialog;

namespace BrownyBot.Dialogs
{
  [Serializable]
  public class GreetingsDialog : BestMatchDialog<bool>
  {
    [BestMatch(new[] { "Hi", "Hi There", "Hello there", "Hey", "Hello",
            "Hey there", "Greetings", "Good morning", "Good afternoon", "Good evening", "Good day" },
       threshold: 0.5, ignoreCase: true, ignoreNonAlphaNumericCharacters: true)]
    public async Task WelcomeGreeting(IDialogContext context, string messageText)
    {
      await context.PostAsync("Hello there. How can I help you?");
      context.Done(true);
    }

    [BestMatch(new[] { "bye", "bye bye", "got to go",
            "see you later", "laters", "adios" })]
    public async Task FarewellGreeting(IDialogContext context, string messageText)
    {
      await context.PostAsync("Bye. Have a good day.");
      context.Done(true);
    }

    public override async Task NoMatchHandler(IDialogContext context, string messageText)
    {
      context.Done(false);
    }
  }
}