namespace UrlMatcher
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    /// <summary>
    /// URL matcher.
    /// </summary>
    public class Matcher
    {
        #region Public-Members

        /// <summary>
        /// URL.
        /// </summary>
        public string Url => _Url;

        /// <summary>
        /// URL parts.
        /// Returns a copy of the internal array to prevent external modification.
        /// </summary>
        public string[] Parts => (string[])_Parts.Clone();

        #endregion

        #region Private-Members

        private string _Url = null;
        private string[] _Parts = null;

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        /// <param name="url">URL.</param>
        public Matcher(string url)
        {
            if (String.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));

            url = StripQueryAndFragment(url);

            _Url = url;
            _Parts = url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        /// <param name="uri">URI.</param>
        public Matcher(Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            string url = StripQueryAndFragment(uri.PathAndQuery);

            _Url = url;
            _Parts = url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion

        #region Public-Methods

        /// <summary>
        /// Match the URL or URI supplied in the constructor against a pattern.
        /// For example, match URI http://localhost:8000/v1.0/something/else/32 against pattern /{v}/something/else/{id}.
        /// Or, match URL /v1.0/something/else/32 against pattern /{v}/something/else/{id}.
        /// If a match exists, vals will contain keys name 'v' and 'id', and the associated values from the supplied URL.
        /// </summary>
        /// <param name="pattern">The pattern used to evaluate the URI. Parameters are specified using {name} syntax.</param>
        /// <param name="vals">Name value collection containing keys and values. Parameter names are case-insensitive. Will be empty if no match. URL-encoded values are matched as-is (not decoded).</param>
        /// <returns>True if matched. Note: Literal parts are case-sensitive while parameter names are case-insensitive.</returns>
        public bool Match(string pattern, out NameValueCollection vals)
        {
            vals = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);
            if (String.IsNullOrEmpty(pattern)) throw new ArgumentNullException(nameof(pattern));
            string[] patternParts = pattern.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            return MatchInternal(_Parts, patternParts, out vals);
        }

        /// <summary>
        /// Match a URI against a pattern.
        /// For example, match URI http://localhost:8000/v1.0/something/else/32 against pattern /{v}/something/else/{id}.
        /// If a match exists, vals will contain keys name 'v' and 'id', and the associated values from the supplied URL.
        /// </summary>
        /// <param name="uri">The URI to evaluate.</param>
        /// <param name="pattern">The pattern used to evaluate the URI. Parameters are specified using {name} syntax.</param>
        /// <param name="vals">Name value collection containing keys and values. Parameter names are case-insensitive. Will be empty if no match.</param>
        /// <returns>True if matched. Note: Literal parts are case-sensitive while parameter names are case-insensitive.</returns>
        public static bool Match(Uri uri, string pattern, out NameValueCollection vals)
        {
            vals = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (String.IsNullOrEmpty(pattern)) throw new ArgumentNullException(nameof(pattern));
            return Match(uri.PathAndQuery, pattern, out vals);
        }

        /// <summary>
        /// Match a URL against a pattern.
        /// For example, match URL /v1.0/something/else/32 against pattern /{v}/something/else/{id}.
        /// If a match exists, vals will contain keys name 'v' and 'id', and the associated values from the supplied URL.
        /// </summary>
        /// <param name="url">The URL to evaluate.</param>
        /// <param name="pattern">The pattern used to evaluate the URL. Parameters are specified using {name} syntax.</param>
        /// <param name="vals">Name value collection containing keys and values. Parameter names are case-insensitive. Will be empty if no match.</param>
        /// <returns>True if matched. Note: Literal parts are case-sensitive while parameter names are case-insensitive.</returns>
        public static bool Match(string url, string pattern, out NameValueCollection vals)
        {
            vals = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);
            if (String.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
            if (String.IsNullOrEmpty(pattern)) throw new ArgumentNullException(nameof(pattern));

            url = StripQueryAndFragment(url);

            string[] urlParts = url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string[] patternParts = pattern.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            return MatchInternal(urlParts, patternParts, out vals);
        }

        #endregion

        #region Private-Methods

        private static bool MatchInternal(string[] urlParts, string[] patternParts, out NameValueCollection vals)
        {
            vals = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);

            if (urlParts.Length != patternParts.Length) return false;

            for (int i = 0; i < urlParts.Length; i++)
            {
                string paramName = ExtractParameter(patternParts[i]);

                if (String.IsNullOrEmpty(paramName))
                {
                    // no pattern - literal match (case-sensitive)
                    if (!urlParts[i].Equals(patternParts[i], StringComparison.Ordinal))
                    {
                        return false;
                    }
                }
                else
                {
                    vals.Add(paramName, urlParts[i]);
                }
            }

            return true;
        }

        private static string ExtractParameter(string pattern)
        {
            if (String.IsNullOrEmpty(pattern)) throw new ArgumentNullException(nameof(pattern));

            int indexStart = pattern.IndexOf('{');
            if (indexStart == -1) return null;

            int indexEnd = pattern.IndexOf('}', indexStart);
            if (indexEnd == -1 || indexEnd <= indexStart + 1) return null;

            // Return content without braces
            return pattern.Substring(indexStart + 1, indexEnd - indexStart - 1);
        }

        private static string StripQueryAndFragment(string url)
        {
            int queryIndex = url.IndexOf('?');
            int fragmentIndex = url.IndexOf('#');

            if (queryIndex == -1 && fragmentIndex == -1) return url;

            int cutIndex = queryIndex;
            if (cutIndex == -1 || (fragmentIndex != -1 && fragmentIndex < cutIndex))
            {
                cutIndex = fragmentIndex;
            }

            return url.Substring(0, cutIndex);
        }

        #endregion
    }
}
