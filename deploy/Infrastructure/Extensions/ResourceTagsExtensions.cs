using Pulumi;

namespace Deploy.Infrastructure.Extensions;

public static class ResourceTagsExtensions
{
    public static Dictionary<string, string> AddTag(this Dictionary<string, string> tags, string key, string value)
    {
        var newDictionary = new Dictionary<string, string>(tags)
        {
            {key, value},
        };
        return newDictionary;
    }

    public static Dictionary<string, string> AddTags(this Dictionary<string, string> tags, Dictionary<string, string> moreTags)
    {
        return tags.Concat(moreTags).ToDictionary(x => x.Key , x => x.Value);
    }
}
