using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace BrownyBot.Services
{
  public class ResponseService
  {
    private readonly ICaptionService _captionService = new CognitiveCaptionService();

    public async Task Process(Activity activity)
    {
      ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

      string response;
      var replyMessage = activity.CreateReply(string.Empty);

      if (ContainsImage(activity))
      {
        try
        {
          response = await GetCaptionAsync(activity, connector);
        }
        catch (ArgumentException e)
        {
          response = "Did you upload an image? I'm more of a visual person. " +
                     "Try sending me an image or an image URL";

          Trace.TraceError(e.ToString());
        }
        catch (Exception e)
        {
          response = "Oops! Something went wrong. Try again later.";

          Trace.TraceError(e.ToString());
        }
      }
      else
      {
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
            replyMessage.Attachments = new List<Attachment> {GetInternetAttachment()};
            break;
          case "who are you":
          case "tell me a bit about yourself":
          case "information":
            response =
              "My name is Browny (because of my warm personality). I am a small-sized combat robot. Officially known as CX-1-DA300. My ability to link up with numerous data systems makes me an unsurpassed intelligence tool.";
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

    private static async Task<Stream> GetImageStream(ConnectorClient connector, Attachment imageAttachment)
    {
      using (var httpClient = new HttpClient())
      {
        // The Skype attachment URLs are secured by JwtToken,
        // you should set the JwtToken of your bot as the authorization header for the GET request your bot initiates to fetch the image.
        // https://github.com/Microsoft/BotBuilder/issues/662
        var uri = new Uri(imageAttachment.ContentUrl);
        if (uri.Host.EndsWith("skype.com") && uri.Scheme == "https")
        {
          httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            await GetTokenAsync(connector));
          httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
        }

        return await httpClient.GetStreamAsync(uri);
      }
    }

    /// <summary>
    /// Gets the href value in an anchor element.
    /// </summary>
    ///  Skype transforms raw urls to html. Here we extract the href value from the url
    /// <param name="text">Anchor tag html.</param>
    /// <param name="url">Url if valid anchor tag, null otherwise</param>
    /// <returns>True if valid anchor element</returns>
    private static bool TryParseAnchorTag(string text, out string url)
    {
      var regex = new Regex("^<a href=\"(?<href>[^\"]*)\">[^<]*</a>$", RegexOptions.IgnoreCase);
      url = regex.Matches(text).OfType<Match>().Select(m => m.Groups["href"].Value).FirstOrDefault();
      return url != null;
    }

    /// <summary>
    /// Gets the JwT token of the bot. 
    /// </summary>
    /// <param name="connector"></param>
    /// <returns>JwT token of the bot</returns>
    private static async Task<string> GetTokenAsync(ConnectorClient connector)
    {
      if (connector.Credentials is MicrosoftAppCredentials credentials)
        return await credentials.GetTokenAsync();

      return null;
    }

    /// <summary>
    /// Gets the caption asynchronously by checking the type of the image (stream vs URL)
    /// and calling the appropriate caption service method.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <param name="connector">The connector.</param>
    /// <returns>The caption if found</returns>
    /// <exception cref="ArgumentException">The activity doesn't contain a valid image attachment or an image URL.</exception>
    private async Task<string> GetCaptionAsync(Activity activity, ConnectorClient connector)
    {
      var imageAttachment = activity.Attachments?.FirstOrDefault(a => a.ContentType.Contains("image"));
      if (imageAttachment != null)
      {
        using (var stream = await GetImageStream(connector, imageAttachment))
        {
          return await _captionService.GetCaptionAsync(stream);
        }
      }

      if (TryParseAnchorTag(activity.Text, out string url))
      {
        return await _captionService.GetCaptionAsync(url);
      }

      if (Uri.IsWellFormedUriString(activity.Text, UriKind.Absolute))
      {
        return await _captionService.GetCaptionAsync(activity.Text);
      }

      // If we reach here then the activity is neither an image attachment nor an image URL.
      throw new ArgumentException("The activity doesn't contain a valid image attachment or an image URL.");
    }

    private bool ContainsImage(Activity activity)
    {
      var imageAttachment = activity.Attachments?.FirstOrDefault(a => a.ContentType.Contains("image"));
      return imageAttachment != null || Uri.IsWellFormedUriString(activity.Text, UriKind.Absolute);
    }
  }
}