using System;
using System.Linq;
using Microsoft.Bot.Connector;

namespace BrownyBot
{
  public static class Utils
  {
    private static Attachment GetInternetAttachment()
    {
      return new Attachment
      {
        Name = "Browny.jpg",
        ContentType = "image/jpg",
        ContentUrl = "http://pre06.deviantart.net/6f35/th/pre/f/2012/185/e/5/brownie_by_090shinsuke090-d560bos.jpg"
      };
    }

    public static bool ContainsImage(IActivity activity)
    {
      var imageAttachment = activity.AsMessageActivity()?.Attachments?.FirstOrDefault(a => a.ContentType.Contains("image"));
      return imageAttachment != null || Uri.IsWellFormedUriString(activity.AsMessageActivity()?.Text, UriKind.Absolute);
    }
  }
}