﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BrownyBot.Dialogs;
using BrownyBot.Internal;
using BrownyBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BrownyBot.Controllers
{
  [BotAuthentication]
  public class MessagesController : ApiController
  {
    /// <summary>
    /// POST: api/Messages
    /// Receive a message from a user and reply to it
    /// </summary>
    public async Task<HttpResponseMessage> Post([FromBody] Activity activity)
    {
      if (activity.Type == ActivityTypes.Message)
      {
        await Conversation.SendAsync(activity, () => new BrownyLuisDialog());
      }
      else
      {
        await HandleSystemMessage(activity);
      }
      var response = Request.CreateResponse(HttpStatusCode.OK);
      return response;
    }

    private async Task HandleSystemMessage(Activity message)
    {
      if (message.Type == ActivityTypes.DeleteUserData)
      {
        // Implement user deletion here
        // If we handle user deletion, return a real message
      }
      else if (message.Type == ActivityTypes.ConversationUpdate)
      { 
        if (message.MembersAdded.Any(x => x.Id == message.Recipient.Id))
        {
          var reply = message.CreateReply(Responses.WelcomeMessage);

          var connector = new ConnectorClient(new Uri(message.ServiceUrl));

          await connector.Conversations.ReplyToActivityAsync(reply);
        }
      }
      else if (message.Type == ActivityTypes.ContactRelationUpdate)
      {
        // Handle add/remove from contact lists
        // Activity.From + Activity.Action represent what happened
      }
      else if (message.Type == ActivityTypes.Typing)
      {
        // Handle knowing tha the user is typing
      }
      else if (message.Type == ActivityTypes.Ping)
      {
      }
    }
  }
}