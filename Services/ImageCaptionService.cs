using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace BrownyBot.Services
{
  public class ImageCaptionService
  {
    private readonly CognitiveCaptionService _captionService = new CognitiveCaptionService();

    private static async Task<Stream> GetImageStream(Attachment imageAttachment)
    {
      using (var httpClient = new HttpClient())
      {
        var uri = new Uri(imageAttachment.ContentUrl);

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
    /// Gets the caption asynchronously by checking the type of the image (stream vs URL)
    /// and calling the appropriate caption service method.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <returns>The caption if found</returns>
    /// <exception cref="ArgumentException">The activity doesn't contain a valid image attachment or an image URL.</exception>
    public async Task<string> GetCaptionAsync(Activity activity)
    {
      var imageAttachment = activity.Attachments?.FirstOrDefault(a => a.ContentType.Contains("image"));
      if (imageAttachment != null)
      {
        using (var stream = await GetImageStream(imageAttachment))
        {
          return await _captionService.GetCaptionAsync(stream);
        }
      }

      string url;
      if (TryParseAnchorTag(activity.Text, out url))
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
  }
}