using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace BrownyBot.Services
{
  public class ResponseService
  {
    public static async Task Process(Activity activity)
    {
      ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

      string response;
      var replyMessage = activity.CreateReply(string.Empty);

      switch (activity.Text.ToLower())
      {
        case "hello":
        case "hi":
          response = "Hi!";
          break;
        case "how are you":
          response = "I'm fine! And you?";
          break;
        case "what do you look like":
          response = "Something like this";
          replyMessage.Attachments = new List<Attachment> { GetInternetAttachment() };
          break;
        case "who are you":
        case "tell me a bit about yourself":
        case "information":
          response = "My name is Browny (because of my warm personality). I am a small-sized combat robot. Officially known as CX-1-DA300. My ability to link up with numerous data systems makes me an unsurpassed intelligence tool.";
          break;
        case "what are your weapons":
        case "what weapons do you have":
        case "how do you fight":
        case "weapons":
          response = "I have four weapons: Victory Laser, Gemini Scatter, Electromagnetic Yo-Yo and Shield Chaser";
          break;
        case "weapon a":
          response = "Weapon A: Victory Laser - A rapid fire laser that shoots a V-shaped beam.";
          break;
        case "weapon b":
          response = "Weapon B: Gemini Scatter - Returns like a boomerang.";
          break;
        case "weapon c":
          response =
            "Weapon C: Electromagnetic Yo-Yo - Holding the button down causes it to automatically attack the enemy.";
          break;
        case "weapon d":
          response =
            "Weapon D: Shield Chaser - Holding the button down causes it to rotate around Browny, and releasing it fires it directly at the nearest enemy.";
          break;

        default:
          response = "Hm...";
          break;
      }
      replyMessage.Text = response;
      await connector.Conversations.ReplyToActivityAsync(replyMessage);
    }

    private static Attachment GetInternetAttachment()
    {
      return new Attachment
      {
        Name = "Browny.jpg",
        ContentType = "image/jpg",
        ContentUrl = "http://pre06.deviantart.net/6f35/th/pre/f/2012/185/e/5/brownie_by_090shinsuke090-d560bos.jpg"
      };
    }
  }
}