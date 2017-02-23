using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BrownyBot.Internal;
using BrownyBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BrownyBot.Dialogs
{
  [Serializable]
  public class CaptionDialog : IDialog<object>
  {
    private bool _active;
    public async Task StartAsync(IDialogContext context)
    {
      await context.PostAsync("Caption mode started");
      context.Wait(MessageReceivedAsync);
    }

    public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
    {
      if (!_active)
      {
        _active = true;
        context.Wait(MessageReceivedAsync);
        return;
      }

      var message = await argument;
      if (message.Text == "return" || message.Text == "back" || message.Text == "exit")
      {
        context.Done(true);
        return;
      }

      if (Utils.ContainsImage(context.Activity))
      {
        string response;
        try
        {
          //TODO DI
          response = await new ImageCaptionService().GetCaptionAsync(context.Activity as Activity);
        }
        catch (ArgumentException e)
        {
          response = Responses.InvalidImage;

          Trace.TraceError(e.ToString());
        }
        catch (Exception e)
        {
          response = Responses.TryAgain;

          Trace.TraceError(e.ToString());
        }

        await context.PostAsync(response);
        context.Wait(MessageReceivedAsync);
      }
      else
      {
        await context.PostAsync(Responses.InvalidImage);
        context.Wait(MessageReceivedAsync);
      }
    }
  }
}