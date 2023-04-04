using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Griffin.Data.Meta;

/// <summary>
///     Summary for the Inflector class
/// </summary>
public class Inflector
{
    private readonly List<InflectorRule> _plurals = new();
    private readonly List<InflectorRule> _singulars = new();
    private readonly List<string> _uncounTableCollection = new();

    /// <summary>
    ///     Initializes the <see cref="Inflector" /> class.
    /// </summary>
    private Inflector()
    {
        AddPluralRule("$", "s");
        AddPluralRule("s$", "s");
        AddPluralRule("(ax|test)is$", "$1es");
        AddPluralRule("(octop|vir)us$", "$1i");
        AddPluralRule("(alias|status)$", "$1es");
        AddPluralRule("(bu)s$", "$1ses");
        AddPluralRule("(buffal|tomat)o$", "$1oes");
        AddPluralRule("([ti])um$", "$1a");
        AddPluralRule("sis$", "ses");
        AddPluralRule("(?:([^f])fe|([lr])f)$", "$1$2ves");
        AddPluralRule("(hive)$", "$1s");
        AddPluralRule("([^aeiouy]|qu)y$", "$1ies");
        AddPluralRule("(x|ch|ss|sh)$", "$1es");
        AddPluralRule("(matr|vert|ind)ix|ex$", "$1ices");
        AddPluralRule("([m|l])ouse$", "$1ice");
        AddPluralRule("^(ox)$", "$1en");
        AddPluralRule("(quiz)$", "$1zes");

        AddSingularRule("s$", string.Empty);
        AddSingularRule("ss$", "ss");
        AddSingularRule("(n)ews$", "$1ews");
        AddSingularRule("([ti])a$", "$1um");
        AddSingularRule("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis");
        AddSingularRule("(^analy)ses$", "$1sis");
        AddSingularRule("([^f])ves$", "$1fe");
        AddSingularRule("(hive)s$", "$1");
        AddSingularRule("(tive)s$", "$1");
        AddSingularRule("([lr])ves$", "$1f");
        AddSingularRule("([^aeiouy]|qu)ies$", "$1y");
        AddSingularRule("(s)eries$", "$1eries");
        AddSingularRule("(m)ovies$", "$1ovie");
        AddSingularRule("(x|ch|ss|sh)es$", "$1");
        AddSingularRule("([m|l])ice$", "$1ouse");
        AddSingularRule("(bus)es$", "$1");
        AddSingularRule("(o)es$", "$1");
        AddSingularRule("(shoe)s$", "$1");
        AddSingularRule("(cris|ax|test)es$", "$1is");
        AddSingularRule("(octop|vir)i$", "$1us");
        AddSingularRule("(alias|status)$", "$1");
        AddSingularRule("(alias|status)es$", "$1");
        AddSingularRule("^(ox)en", "$1");
        AddSingularRule("(vert|ind)ices$", "$1ex");
        AddSingularRule("(matr)ices$", "$1ix");
        AddSingularRule("(quiz)zes$", "$1");

        AddIrregularRule("person", "people");
        AddIrregularRule("man", "men");
        AddIrregularRule("child", "children");
        AddIrregularRule("sex", "sexes");
        AddIrregularRule("tax", "taxes");
        AddIrregularRule("move", "moves");

        AddUnknownCountRule("equipment");
        AddUnknownCountRule("information");
        AddUnknownCountRule("rice");
        AddUnknownCountRule("money");
        AddUnknownCountRule("species");
        AddUnknownCountRule("series");
        AddUnknownCountRule("fish");
        AddUnknownCountRule("sheep");
    }

    /// <summary>
    ///     Current instance.
    /// </summary>
    public static Inflector Instance { get; } = new();

    /// <summary>
    ///     Adds the irregular rule.
    /// </summary>
    /// <param name="singular">The singular.</param>
    /// <param name="plural">The plural.</param>
    private void AddIrregularRule(string singular, string plural)
    {
        AddPluralRule(string.Concat("(", singular[0], ")", singular[1..], "$"),
            string.Concat("$1", plural[1..]));
        AddSingularRule(string.Concat("(", plural[0], ")", plural[1..], "$"),
            string.Concat("$1", singular[1..]));
    }

    /// <summary>
    ///     Adds the unknown count rule.
    /// </summary>
    /// <param name="word">The word.</param>
    private void AddUnknownCountRule(string word)
    {
        _uncounTableCollection.Add(word.ToLower());
    }

    /// <summary>
    ///     Adds the plural rule.
    /// </summary>
    /// <param name="rule">The rule.</param>
    /// <param name="replacement">The replacement.</param>
    private void AddPluralRule(string rule, string replacement)
    {
        _plurals.Add(new InflectorRule(rule, replacement));
    }

    /// <summary>
    ///     Adds the singular rule.
    /// </summary>
    /// <param name="rule">The rule.</param>
    /// <param name="replacement">The replacement.</param>
    private void AddSingularRule(string rule, string replacement)
    {
        _singulars.Add(new InflectorRule(rule, replacement));
    }

    /// <summary>
    ///     Makes the plural.
    /// </summary>
    /// <param name="word">The word.</param>
    /// <returns></returns>
    public string MakePlural(string word)
    {
        return ApplyRules(_plurals, word);
    }

    /// <summary>
    ///     Makes the singular.
    /// </summary>
    /// <param name="word">The word.</param>
    /// <returns></returns>
    public string MakeSingular(string word)
    {
        return ApplyRules(_singulars, word);
    }

    /// <summary>
    ///     Applies the rules.
    /// </summary>
    /// <param name="rules">The rules.</param>
    /// <param name="word">The word.</param>
    /// <returns></returns>
    private string ApplyRules(IList<InflectorRule> rules, string word)
    {
        var result = word;
        if (!_uncounTableCollection.Contains(word.ToLower()))
            for (var i = rules.Count - 1; i >= 0; i--)
            {
                var currentPass = rules[i].Apply(word);
                if (currentPass != null)
                {
                    result = currentPass;
                    break;
                }
            }

        return result;
    }

    /// <summary>
    ///     Converts the string to title case.
    /// </summary>
    /// <param name="word">The word.</param>
    /// <returns></returns>
    public string ToTitleCase(string word)
    {
        return Regex.Replace(ToHumanCase(AddUnderscores(word)), @"\b([a-z])",
            delegate(Match match) { return match.Captures[0].Value.ToUpper(); });
    }

    /// <summary>
    ///     Converts the string to human case.
    /// </summary>
    /// <param name="lowercaseAndUnderscoredWord">The lowercase and underscored word.</param>
    /// <returns></returns>
    public string ToHumanCase(string lowercaseAndUnderscoredWord)
    {
        return MakeInitialCaps(Regex.Replace(lowercaseAndUnderscoredWord, @"_", " "));
    }


    /// <summary>
    ///     Adds the underscores.
    /// </summary>
    /// <param name="pascalCasedWord">The pascal cased word.</param>
    /// <returns></returns>
    public string AddUnderscores(string pascalCasedWord)
    {
        return
            Regex.Replace(
                Regex.Replace(Regex.Replace(pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1_$2"), @"([a-z\d])([A-Z])",
                    "$1_$2"), @"[-\s]", "_").ToLower();
    }

    /// <summary>
    ///     Makes the initial caps.
    /// </summary>
    /// <param name="word">The word.</param>
    /// <returns></returns>
    public string MakeInitialCaps(string word)
    {
        return string.Concat(word.Substring(0, 1).ToUpper(), word.Substring(1).ToLower());
    }

    /// <summary>
    ///     Makes the initial lower case.
    /// </summary>
    /// <param name="word">The word.</param>
    /// <returns></returns>
    public string MakeInitialLowerCase(string word)
    {
        return string.Concat(word.Substring(0, 1).ToLower(), word.Substring(1));
    }


    /// <summary>
    ///     Determine whether the passed string is numeric, by attempting to parse it to a double
    /// </summary>
    /// <param name="str">The string to evaluated for numeric conversion</param>
    /// <returns>
    ///     <c>true</c> if the string can be converted to a number; otherwise, <c>false</c>.
    /// </returns>
    public bool IsStringNumeric(string str)
    {
        return double.TryParse(str, NumberStyles.Float, NumberFormatInfo.CurrentInfo, out _);
    }

    /// <summary>
    ///     Adds the ordinal suffix.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <returns></returns>
    public string AddOrdinalSuffix(string number)
    {
        if (IsStringNumeric(number))
        {
            var n = int.Parse(number);
            var nMod100 = n % 100;

            if (nMod100 >= 11 && nMod100 <= 13)
                return string.Concat(number, "th");

            switch (n % 10)
            {
                case 1:
                    return string.Concat(number, "st");
                case 2:
                    return string.Concat(number, "nd");
                case 3:
                    return string.Concat(number, "rd");
                default:
                    return string.Concat(number, "th");
            }
        }

        return number;
    }

    /// <summary>
    ///     Converts the underscores to dashes.
    /// </summary>
    /// <param name="underscoredWord">The underscored word.</param>
    /// <returns></returns>
    public string ConvertUnderscoresToDashes(string underscoredWord)
    {
        return underscoredWord.Replace('_', '-');
    }

    #region Nested type: InflectorRule

    /// <summary>
    ///     Summary for the InflectorRule class
    /// </summary>
    private class InflectorRule
    {
        private readonly Regex _regex;
        private readonly string _replacement;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InflectorRule" /> class.
        /// </summary>
        /// <param name="regexPattern">The regex pattern.</param>
        /// <param name="replacementText">The replacement text.</param>
        public InflectorRule(string regexPattern, string replacementText)
        {
            _regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
            _replacement = replacementText;
        }

        /// <summary>
        ///     Applies the specified word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public string? Apply(string word)
        {
            if (!_regex.IsMatch(word))
                return null;

            var replace = _regex.Replace(word, _replacement);
            if (word == word.ToUpper())
                replace = replace.ToUpper();

            return replace;
        }
    }

    #endregion
}