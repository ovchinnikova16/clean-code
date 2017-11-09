using System.Collections.Generic;
using NUnit.Framework;
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
            var strongTags = new Stack<Tuple<Tag, Tag>>();
            var stackTag = new Stack<Tag>();

            for (var i = 0; i < line.Length; i++)
            {
                if (line[i] == '\\')
                {
                    line = line.Remove(i, 1);
                    i++;
                }
                var tag = Tag.CreateTag(line, i);
                if (tag == null)
                    continue;
                if (tag.Type == Tag.TagType.Strong)
                    i++;

                if (Tag.IsRightClosingTag(line, tag, stackTag))
                {
                    var openingTag = stackTag.Pop();
                    while (openingTag.Type != tag.Type && stackTag.Count != 0)
                        openingTag = stackTag.Pop();
                    var closingTag = tag;

                    if (closingTag.Type == Tag.TagType.Strong && stackTag.Any(x => x.Type == Tag.TagType.Em))
                        strongTags.Push(new Tuple<Tag, Tag>(openingTag, closingTag));
                    else
                    {
                        if (closingTag.Type == Tag.TagType.Em)
                            strongTags.Clear();
                        line = ReplaceTag(line, openingTag, closingTag);
                    }
                }

                else if (Tag.IsRightOpeningTag(line, tag))
                {
                    if (tag.Type == Tag.TagType.Em)
                    {
                        while (strongTags.Count > 0)
                        {
                            var tagsPair = strongTags.Pop();
                            line = ReplaceTag(line, tagsPair.Item1, tagsPair.Item2);
                        }
                    }
                    stackTag.Push(tag);
                }
            }
            while (strongTags.Count > 0)
            {
                var tagsPair = strongTags.Pop();
                line = ReplaceTag(line, tagsPair.Item1, tagsPair.Item2);
            }
            return line;
        }

        public static string ReplaceTag(string line, Tag openingTag, Tag closingTag)
        {
            var length = 1;
            if (openingTag.Type == Tag.TagType.Strong)
                length++;
            closingTag.Position += HtmlTags[closingTag.Type].Length - length;
            return line
                .Remove(openingTag.Position, length)
                .Insert(openingTag.Position, HtmlTags[openingTag.Type])
                .Remove(closingTag.Position, length)
                .Insert(closingTag.Position, HtmlTags[openingTag.Type].Insert(1, "/"));
        }

        public static Dictionary<Tag.TagType, string> HtmlTags = new Dictionary<Tag.TagType, string>
        {
            {Tag.TagType.Strong, "<strong>"},
            {Tag.TagType.Em, "<em>" },

        };
    }
}