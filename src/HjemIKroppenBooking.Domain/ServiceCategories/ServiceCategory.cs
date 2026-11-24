using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.ServiceTypes;

namespace HjemIKroppenBooking.Domain.ServiceCategories;

/// <summary>
/// Main category of a <see cref="ServiceType"/>, e.g. dance therapy or group therapy. Managed by admins, who only set the
/// name. Categories are listed in the order they were created.
/// </summary>
public sealed partial class ServiceCategory : Entity
{
    public const int NameMaxLength = 100;
    public const int SlugMaxLength = 100;

    /// <summary>Leaves room for a "-2", "-3" … suffix when two names give the same slug.</summary>
    private const int SlugBaseMaxLength = SlugMaxLength - 10;

    private const string FallbackSlug = "kategori";

    private ServiceCategory()
    {
    }

    /// <summary>Display name. Unique.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// URL key made from the name, e.g. <c>danseterapi</c>. Used by the frontend in links such as
    /// <c>/book?kategori=danseterapi</c>. Unique, never entered by admins.
    /// </summary>
    public string Slug { get; private set; } = string.Empty;

    /// <param name="slug">Use <see cref="SlugFrom"/>, plus a suffix if needed to make it unique.</param>
    public static ServiceCategory Create(string name, string slug)
    {
        var category = new ServiceCategory();
        category.Rename(name, slug);
        return category;
    }

    /// <param name="slug">Use <see cref="SlugFrom"/>, plus a suffix if needed to make it unique.</param>
    public void Rename(string name, string slug)
    {
        var validName = Guard.RequiredText(name, NameMaxLength, "name");
        var validSlug = Guard.RequiredText(slug, SlugMaxLength, "slug");
        if (!SlugPattern().IsMatch(validSlug))
        {
            throw DomainException.Validation(ErrorCodes.ValidationSlugInvalid, Guard.Field("slug"));
        }

        Name = validName;
        Slug = validSlug;
    }

    /// <summary>Makes a URL key from a Danish name, e.g. "Dans &amp; bevægelse" → <c>dans-bevaegelse</c>.</summary>
    public static string SlugFrom(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        var text = name.Trim().ToLowerInvariant()
            .Replace("æ", "ae", StringComparison.Ordinal)
            .Replace("ø", "oe", StringComparison.Ordinal)
            .Replace("å", "aa", StringComparison.Ordinal)
            .Normalize(NormalizationForm.FormD);

        var slug = new StringBuilder(text.Length);
        var pendingHyphen = false;
        foreach (var c in text)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (c is (>= 'a' and <= 'z') or (>= '0' and <= '9'))
            {
                if (pendingHyphen && slug.Length > 0)
                {
                    slug.Append('-');
                }

                slug.Append(c);
                pendingHyphen = false;
            }
            else
            {
                pendingHyphen = true;
            }
        }

        var result = slug.Length > SlugBaseMaxLength ? slug.ToString(0, SlugBaseMaxLength).TrimEnd('-') : slug.ToString();
        return result.Length == 0 ? FallbackSlug : result;
    }

    [GeneratedRegex("^[a-z0-9]+(-[a-z0-9]+)*$", RegexOptions.CultureInvariant)]
    private static partial Regex SlugPattern();
}
