using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown
{
    public class Tag
    {
        public int Position;
        public TagType Type;

        public Tag(int pos, TagType type)
        {
            Position = pos;
            Type = type;
        }

        public static Tag CreateTag(string line, int pos)
        {
            if (line[pos] == '_')
            {
                if (pos < line.Length - 1 && line[pos + 1] == '_')
                    return new Tag(pos, TagType.Strong);
                return new Tag(pos, TagType.Em);
            }
            return null;
        }

        public static bool IsRightOpeningTag(string line, Tag tag)
        {
            return tag.Position < line.Length - 1
                && line[tag.Position + 1] != ' '
                && !WithDigits(line, tag.Position);
        }

        public static bool IsRightClosingTag(string line, Tag tag, Stack<Tag> stackTag)
        {
            if (tag.Position > 0 && line[tag.Position - 1] != ' ' && !WithDigits(line, tag.Position))
                foreach (var t in stackTag)
                    if (t.Type == tag.Type)
                        return true;
            return false;
        }

        public static bool WithDigits(string line, int pos)
        {
            return (pos + 1 < line.Length && char.IsDigit(line[pos + 1])) ||
                (pos - 1 >= 0 && char.IsDigit(line[pos - 1]));
        }

        public enum TagType
        {
            Strong,
            Em
        }
    }
}
