using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace AgilityPackGenerateHeaderAnchors.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var sampleHtml = GetSampleHtml();
            var htmlWithAnchors = AddHeadingAnchorsToHtml(sampleHtml);
            return Content(htmlWithAnchors, "text/html");
        }

        public string AddHeadingAnchorsToHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            // select all possible headings in the document
            var headings = doc.DocumentNode.SelectNodes("//h1 | //h2 | //h3 | //h4 | //h5 | //h6");
            if (headings != null)
            {
                foreach (var heading in headings)
                {
                    var headingText = heading.InnerText;
                    // if heading has id, use it
                    string headingId = heading.Attributes["id"]?.Value;
                    if (headingId == null)
                    {
                        // if heading does not have an id, generate a safe id by creating a slug based on the heading text
                        // slug is a URL/SEO friendly part of a URL, this is a good option for generating anchor fragments
                        headingId = ToSlug(headingText);
                        // for the fragment to work (jump to the relevant content), the heading id and fragment needs to match
                        heading.Attributes.Append("id", headingId);
                    }
                    
                    // use a non-breaking space to make sure the heading text and the #-sign don't appear on a separate line
                    heading.InnerHtml += "&nbsp;";
                    // create the heading anchor which points to the heading
                    var headingAnchor = HtmlNode.CreateNode($"<a href=\"#{headingId}\" aria-label=\"Anchor for heading: {headingText}\">#</a>");
                    // append the anchor behind the heading text content
                    heading.AppendChild(headingAnchor);
                }
            }

            return doc.DocumentNode.InnerHtml;
        }

        public static string ToSlug(string phrase)
        {
            var str = phrase.ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        public string GetSampleHtml()
        {
            var sampleHtmlFilePath = $"{Server.MapPath("~")}/SampleHtml.html";
            return System.IO.File.ReadAllText(sampleHtmlFilePath);
        }
    }
}