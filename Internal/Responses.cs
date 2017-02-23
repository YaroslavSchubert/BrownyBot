using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrownyBot.Internal
{
  internal static class Responses
  {
    public const string AboutBrowny = "My name is Browny (because of my warm personality)."
                                      + " I am a small-sized combat robot. Officially known as CX-1-DA300."
                                      +
                                      " My ability to link up with numerous data systems makes me an unsurpassed intelligence tool.";

    public const string AboutAuthor = "Yaroslav Schubert. Software engineer";

    public const string WelcomeMessage =
      "Hi \n\n"
      + "I am Browny, small-sized combat robot from Contra Hard Corps.  \n"
      + "Currently I have following features  \n"
      + Features + "\n"
      + "You can type 'Help' to get this information again";

    public const string Features = "~ Tell the story of my life: Try 'Who are you'\n\n"
                                   + "~ Answer question about the author: Try 'Who is author'\n\n"
                                   + "~ Image caption: Try send a picture or a link to a picture \n\n"
                                   + "~ Answer about the weather: Try 'Weather Kyiv now'";

    public const string Help =
      "I can do the following   \n"
      + Features
      + "What would you like me to do?";

    public const string TryAgain = "Oops! Something went wrong. Try again later.";

    public const string InvalidImage = "Did you upload an image? I'm more of a visual person. " +
                                       "Try sending me an image or an image URL";
  }
}