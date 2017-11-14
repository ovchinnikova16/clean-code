using System.Collections.Generic;
using System;
using System.Linq;

namespace Markdown
{
	public class Md
	{
		public string RenderToHtml(string markdown)
		{
            var resultText = new List<string>();
            foreach (var line in markdown.Split('\n'))
                resultText.Add(ConvertToHtml(line));
            return string.Join("\n", resultText);
		}

        public static string ConvertToHtml(string line)
        {
            var strongTagsPairs = new Stack<Tuple<Tag, Tag>>();
            var openingTags = new Stack<Tag>();

            var pos = 0;
            while (pos < line.Length)
            {
                if (line[pos] == '\\')
                {
                    line = line.Remove(pos, 1);
                    pos++;
                }
                var tag = Tag.CreateTag(line, pos);
                if (tag == null)
                {
                    pos++;
                    continue;
                }
                if (tag.Type == TagType.Strong)
                    pos++;

                if (Tag.IsValidClosingTag(line, tag, openingTags))
                {
                    var closingTag = tag;
                    var openingTag = openingTags.Pop();
                    while (openingTag.Type != tag.Type && openingTags.Any())
                        openingTag = openingTags.Pop();

                    if (IsStrongInEm(closingTag, openingTags))
                        strongTagsPairs.Push(new Tuple<Tag, Tag>(openingTag, closingTag));
                    else
                    {
                        if (closingTag.Type == TagType.Em)
                            strongTagsPairs.Clear();
                        line = ReplaceTag(line, openingTag, closingTag);
                    }
    
                }

                else if (Tag.IsValidOpeningTag(line, tag))
                {
                    if (tag.Type == TagType.Em)
                    {
                        while (strongTagsPairs.Count > 0)
                        {
                            var tagsPair = strongTagsPairs.Pop();
                            line = ReplaceTag(line, tagsPair.Item1, tagsPair.Item2);
                        }
                    }
                    openingTags.Push(tag);
                }
                pos++;
            }

            while (strongTagsPairs.Count > 0)
            {
                var tagsPair = strongTagsPairs.Pop();
                line = ReplaceTag(line, tagsPair.Item1, tagsPair.Item2);
            }
            return line;
        }

	    private static bool IsStrongInEm(Tag closingTag, Stack<Tag> openingTags)
	    {
	        return closingTag.Type == TagType.Strong && openingTags.Any(x => x.Type == TagType.Em);
	    }

	    public static string ReplaceTag(string line, Tag openingTag, Tag closingTag)
        {
            var length = 1;
            if (openingTag.Type == TagType.Strong)
                length++;
            closingTag.Position += HtmlTags[closingTag.Type].Length - length;
            return line
                .Remove(openingTag.Position, length)
                .Insert(openingTag.Position, HtmlTags[openingTag.Type])
                .Remove(closingTag.Position, length)
                .Insert(closingTag.Position, HtmlTags[openingTag.Type].Insert(1, "/"));
        }

        public static Dictionary<TagType, string> HtmlTags = new Dictionary<TagType, string>
        {
            {TagType.Strong, "<strong>"},
            {TagType.Em, "<em>" },

        };
    }
}