using Peregrine.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

// unclosed Xml tags (in comments)
#pragma warning disable 1570

namespace Peregrine.WPF.View.Controls
{
    /// <summary>
    /// An extended TextBlock class, that will take account of formatting tags (similar to Html tags) within the text. 
    /// </summary>
    /// <remarks>
    /// Supported tags
    ///     <b>     bold
    ///     <u>     underline
    ///     <i>     italic
    ///     <fs>    font size      use * to indicate a value relative to the current font size
    ///     <ff>    font family
    ///     <fg>    foreground     } supports named colours, #RRGGBB & #AARRGGBB formats
    ///     <bg>    background     }
    ///     <sub>   subscript
    ///     <sup>   superscript
    ///     <lb>    line-break
    ///     <lt>    <
    ///     <gt>    >
    ///     <sl>    /
    /// </remarks>
    public class perFormattedTextBlock : TextBlock
    {
        public perFormattedTextBlock()
        {
            TextWrapping = TextWrapping.Wrap;
        }

        /// <summary>
        /// Replace the original TextBlock.Text property
        /// 
        /// We can't just override the original property metadata as clearing the Inlines will set TextBlock.Text to an empty string.
        /// </summary>
        public new string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public new static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(perFormattedTextBlock), new FrameworkPropertyMetadata(null, OnTextChanged));

        private static void OnTextChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = (perFormattedTextBlock)source;
            if (textBlock == null)
                return;

            textBlock.Inlines.Clear();

            var newText = e.NewValue as string;
            if (string.IsNullOrEmpty(newText))
                return;

            var itemStack = ParseFormattedText(textBlock);
            GenerateFormattedText(textBlock, itemStack);
        }

        /// <summary>
        /// Should text underline use the current foreground (true) or the control's default foreground (false) 
        /// </summary>
        public bool UnderlineUsesCurrentForegroundBrush
        {
            get => (bool)GetValue(UnderlineUsesCurrentForegroundBrushProperty);
            set => SetValue(UnderlineUsesCurrentForegroundBrushProperty, value);
        }

        public static readonly DependencyProperty UnderlineUsesCurrentForegroundBrushProperty =
            DependencyProperty.Register("UnderlineUsesCurrentForegroundBrush", typeof(bool), typeof(perFormattedTextBlock), new PropertyMetadata(true, OnUnderlineUsesForegroundBrushChanged));

        private static void OnUnderlineUsesForegroundBrushChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (source is perFormattedTextBlock textBlock)
                OnTextChanged(textBlock, new DependencyPropertyChangedEventArgs(TextProperty, null, textBlock.Text));
        }

        /// <summary>
        /// Parse the formatted perFormattedTextBlock.Text into a stack of items
        /// </summary>
        /// <param name="textBlock"></param>
        /// <returns></returns>
        private static Stack<perFormattedTextItem> ParseFormattedText(perFormattedTextBlock textBlock)
        {
            var result = new Stack<perFormattedTextItem>();

            SplitTextAtFirstTag(result,
                textBlock.Text,
                textBlock.FontWeight >= FontWeights.Bold,
                textBlock.TextDecorations.Any(td => td.Location == TextDecorationLocation.Underline),
                textBlock.FontStyle == FontStyles.Italic,
                textBlock.FontSize,
                textBlock.FontFamily,
                textBlock.Foreground,
                textBlock.Background,
                BaselineAlignment.Baseline);

            return result;
        }

        /// <summary>
        /// Split the current text string by the first matched tag
        /// </summary>
        /// <remarks>
        /// Split is either
        ///     aaaaaa<tag>bbbbbb
        /// or  aaaaaa<tag>bbbbbb</tag>cccccc
        ///  
        /// The appropriate attributes for the tag will be applied to whole of part bbbbbb in both cases.
        /// 
        /// Then recurse, looking for additional tags in parts bbbbbb & cccccc as applicable.
        /// By rule, part aaaaaa will just be a plain text item as it contains no opening tags.
        /// 
        /// Note that unlike HTML, when a close tag is reached, it will revert all text attributes to the
        /// state prior to corresponding open tag. Any other tags (without matching close tags) will
        /// effectively be closed too.
        /// i.e. <b>aaa<i>bbb</b>ccc is equivalent to <b>aaa<i>bbb</i></b>ccc
        /// </remarks>
        /// <param name="itemStack"></param>
        /// <param name="text"></param>
        /// <param name="isBold"></param>
        /// <param name="isUnderline"></param>
        /// <param name="isItalic"></param>
        /// <param name="fontSize"></param>
        /// <param name="fontFamily"></param>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <param name="baselineAlignment"></param>
        /// <returns></returns>
        private static void SplitTextAtFirstTag(Stack<perFormattedTextItem> itemStack, string text, bool isBold, bool isUnderline, bool isItalic, double fontSize, FontFamily fontFamily, Brush foreground, Brush background, BaselineAlignment baselineAlignment)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var firstTag = FindFirstTagInText(text);

            // If no opening tag is found then add the whole text in the current style.
            if (firstTag == TagType.None)
            {
                itemStack.Push(new perFormattedTextItem(text, isBold, isUnderline, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment));
                return;
            }

            text.RegexSplitTextIncludingMatch(TagTypeConstants.GetOpeningTag(firstTag), out var textBeforeOpenTag, out var matchedText, out var textAfterOpenTag);

            // push the elements onto the stack in reverse order
            if (firstTag == TagType.LineBreak)
            {
                SplitTextAtFirstTag(itemStack, textAfterOpenTag, isBold, isUnderline, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment);
                itemStack.Push(new perFormattedTextItem
                {
                    IsLineBreak = true
                });
            }
            else if (firstTag == TagType.LessThan)
            {
                SplitTextAtFirstTag(itemStack, textAfterOpenTag, isBold, isUnderline, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment);
                itemStack.Push(new perFormattedTextItem("<", isBold, isUnderline, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment));
            }
            else if (firstTag == TagType.GreaterThan)
            {
                SplitTextAtFirstTag(itemStack, textAfterOpenTag, isBold, isUnderline, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment);
                itemStack.Push(new perFormattedTextItem(">", isBold, isUnderline, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment));
            }
            else if (firstTag == TagType.Slash)
            {
                SplitTextAtFirstTag(itemStack, textAfterOpenTag, isBold, isUnderline, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment);
                itemStack.Push(new perFormattedTextItem("/", isBold, isUnderline, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment));
            }
            else
            {
                var hasClosingTag = textAfterOpenTag.RegexSplitTextExcludingMatch(TagTypeConstants.GetClosingTag(firstTag), out var textBetweenOpenAndCloseTag, out var textAfterCloseTag);

                // Handle the case where there are nested opening tags of the same type.
                var previousOpeningTagCount = -1;
                int openingTagCount;

                // How many more occurrences of the same open tag before the matched closing tag
                // The while loop allows for the case when reaching the calculated closing tag includes additional opening tags.
                while (hasClosingTag
                       && (openingTagCount = textBetweenOpenAndCloseTag.CountRegexMatches(TagTypeConstants.GetOpeningTag(firstTag))) != previousOpeningTagCount)
                {
                    // Redo the search in after text, but skip over the first openingTagCount occurrences of the closing tag this time.
                    // Note that this may result in an appropriate closing tag not being found.
                    if (openingTagCount > 0)
                        hasClosingTag = textAfterOpenTag.RegexSplitTextExcludingMatchWithSkip(TagTypeConstants.GetClosingTag(firstTag), openingTagCount, out textBetweenOpenAndCloseTag, out textAfterCloseTag);

                    previousOpeningTagCount = openingTagCount;
                }

                if (firstTag == TagType.Bold)
                {
                    if (hasClosingTag)
                    {
                        SplitTextAtFirstTag(itemStack, textAfterCloseTag, isBold, isUnderline, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment);
                        SplitTextAtFirstTag(itemStack, textBetweenOpenAndCloseTag, true, isUnderline, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment);
                    }
                    else
                        SplitTextAtFirstTag(itemStack, textAfterOpenTag, true, isUnderline, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment);
                }
                else if (firstTag == TagType.Italic)
                {
                    if (hasClosingTag)
                    {
                        SplitTextAtFirstTag(itemStack, textAfterCloseTag, isBold, isUnderline, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment);
                        SplitTextAtFirstTag(itemStack, textBetweenOpenAndCloseTag, isBold, isUnderline, true, fontSize, fontFamily, foreground, background, baselineAlignment);
                    }
                    else
                        SplitTextAtFirstTag(itemStack, textAfterOpenTag, isBold, isUnderline, true, fontSize, fontFamily, foreground, background, baselineAlignment);
                }
                else if (firstTag == TagType.Underline)
                {
                    if (hasClosingTag)
                    {
                        SplitTextAtFirstTag(itemStack, textAfterCloseTag, isBold, isUnderline, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment);
                        SplitTextAtFirstTag(itemStack, textBetweenOpenAndCloseTag, isBold, true, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment);
                    }
                    else
                        SplitTextAtFirstTag(itemStack, textAfterOpenTag, isBold, true, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment);
                }
                else if (firstTag == TagType.Subscript)
                {
                    if (hasClosingTag)
                    {
                        SplitTextAtFirstTag(itemStack, textAfterCloseTag, isBold, isUnderline, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment);
                        SplitTextAtFirstTag(itemStack, textBetweenOpenAndCloseTag, isBold, isUnderline, isItalic, fontSize * 0.67, fontFamily, foreground, background, BaselineAlignment.Subscript);
                    }
                    else
                        SplitTextAtFirstTag(itemStack, textAfterOpenTag, isBold, isUnderline, isItalic, fontSize / 2, fontFamily, foreground, background, BaselineAlignment.Subscript);
                }
                else if (firstTag == TagType.SuperScript)
                {
                    if (hasClosingTag)
                    {
                        SplitTextAtFirstTag(itemStack, textAfterCloseTag, isBold, isUnderline, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment);

                        // BaselineAlignment.TextTop works better than BaselineAlignment.Superscript
                        SplitTextAtFirstTag(itemStack, textBetweenOpenAndCloseTag, isBold, isUnderline, isItalic, fontSize * 0.67, fontFamily, foreground, background, BaselineAlignment.TextTop);
                    }
                    else
                        SplitTextAtFirstTag(itemStack, textAfterOpenTag, isBold, isUnderline, isItalic, fontSize / 2, fontFamily, foreground, background, BaselineAlignment.TextTop);
                }
                else if (firstTag == TagType.FontSize)
                {
                    var relativeSize = matchedText.Contains('*');

                    var fontSizeText = matchedText.ToLower()
                        .Replace("<", string.Empty)
                        .Replace(TagTypeConstants.GetTag(firstTag), string.Empty)
                        .Replace("=", string.Empty)
                        .Replace("*", string.Empty)
                        .Replace(">", string.Empty)
                        .Trim();

                    double newFontSize;

                    if (relativeSize)
                    {
                        if (!double.TryParse(fontSizeText, out var relativeFontSize))
                            relativeFontSize = 1.0d;

                        newFontSize = relativeFontSize * fontSize;
                    }
                    else if (!double.TryParse(fontSizeText, out newFontSize))
                        newFontSize = fontSize;

                    if (hasClosingTag)
                    {
                        SplitTextAtFirstTag(itemStack, textAfterCloseTag, isBold, isUnderline, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment);
                        SplitTextAtFirstTag(itemStack, textBetweenOpenAndCloseTag, isBold, isUnderline, isItalic, newFontSize, fontFamily, foreground, background, baselineAlignment);
                    }
                    else
                        SplitTextAtFirstTag(itemStack, textAfterOpenTag, isBold, isUnderline, isItalic, newFontSize, fontFamily, foreground, background, baselineAlignment);
                }
                else if (firstTag == TagType.FontFamily)
                {
                    FontFamily newFontFamily;
                    var fontFamilyText = matchedText.ToLower()
                        .Replace("<", string.Empty)
                        .Replace(TagTypeConstants.GetTag(firstTag), string.Empty)
                        .Replace("=", string.Empty)
                        .Replace(">", string.Empty)
                        .Trim();

                    try
                    {
                        newFontFamily = new FontFamily(fontFamilyText);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        newFontFamily = fontFamily;
                    }

                    if (hasClosingTag)
                    {
                        SplitTextAtFirstTag(itemStack, textAfterCloseTag, isBold, isUnderline, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment);
                        SplitTextAtFirstTag(itemStack, textBetweenOpenAndCloseTag, isBold, isUnderline, isItalic, fontSize, newFontFamily, foreground, background, baselineAlignment);
                    }
                    else
                        SplitTextAtFirstTag(itemStack, textAfterOpenTag, isBold, isUnderline, isItalic, fontSize, newFontFamily, foreground, background, baselineAlignment);
                }
                else if (firstTag == TagType.Foreground)
                {
                    Brush newForeground;
                    var foregroundText = matchedText.ToLower().Replace("<", string.Empty)
                        .Replace(TagTypeConstants.GetTag(firstTag), string.Empty)
                        .Replace("=", string.Empty)
                        .Replace(">", string.Empty)
                        .Trim();

                    try
                    {
                        var newForegroundColor = ColorConverter.ConvertFromString(foregroundText);
                        newForeground = newForegroundColor == null
                            ? foreground
                            : new SolidColorBrush((Color)newForegroundColor);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        newForeground = foreground;
                    }

                    if (hasClosingTag)
                    {
                        SplitTextAtFirstTag(itemStack, textAfterCloseTag, isBold, isUnderline, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment);
                        SplitTextAtFirstTag(itemStack, textBetweenOpenAndCloseTag, isBold, isUnderline, isItalic, fontSize, fontFamily, newForeground, background, baselineAlignment);
                    }
                    else
                        SplitTextAtFirstTag(itemStack, textAfterOpenTag, isBold, isUnderline, isItalic, fontSize, fontFamily, newForeground, background, baselineAlignment);
                }
                else if (firstTag == TagType.Background)
                {
                    Brush newBackground;
                    var backgroundText = matchedText.ToLower()
                        .Replace("<", string.Empty)
                        .Replace(TagTypeConstants.GetTag(firstTag), string.Empty)
                        .Replace("=", string.Empty)
                        .Replace(">", string.Empty)
                        .Trim();

                    try
                    {
                        var newBackgroundColor = ColorConverter.ConvertFromString(backgroundText);
                        newBackground = newBackgroundColor == null
                            ? background
                            : new SolidColorBrush((Color)newBackgroundColor);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        newBackground = background;
                    }

                    if (hasClosingTag)
                    {
                        SplitTextAtFirstTag(itemStack, textAfterCloseTag, isBold, isUnderline, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment);
                        SplitTextAtFirstTag(itemStack, textBetweenOpenAndCloseTag, isBold, isUnderline, isItalic, fontSize, fontFamily, foreground, newBackground, baselineAlignment);
                    }
                    else
                        SplitTextAtFirstTag(itemStack, textAfterOpenTag, isBold, isUnderline, isItalic, fontSize, fontFamily, foreground, newBackground, baselineAlignment);
                }
            }

            // Any text before the first matched tag must by rule be plain text in the current style
            itemStack.Push(new perFormattedTextItem(textBeforeOpenTag, isBold, isUnderline, isItalic, fontSize, fontFamily, foreground, background, baselineAlignment));
        }

        /// <summary>
        /// Find the first formatting tag (if any) in the input string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        // 
        private static TagType FindFirstTagInText(string input)
        {
            input = input.ToLower();

            var tagIndexes = perEnumHelper.GetValues<TagType>()
                .Where(tt => tt != TagType.None)
                .Select(tt => new { TagType = tt, FirstIndex = GetIndexOfOpeningTag(input, tt) })
                .Where(x => x.FirstIndex >= 0)
                .OrderBy(x => x.FirstIndex)
                .ToList();

            return tagIndexes.Any()
                       ? tagIndexes.First().TagType
                       : TagType.None;
        }

        /// <summary>
        /// Return the index of the first occurrence of the specified tag in the input string, or -1 if not present. 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="tagType"></param>
        /// <returns></returns>
        private static int GetIndexOfOpeningTag(string input, TagType tagType)
        {
            var openingTag = TagTypeConstants.GetOpeningTag(tagType);
            return input.RegexSplitTextExcludingMatch(openingTag, out var textBeforeMatch, out _)
                ? textBeforeMatch.Length
                : -1;
        }

        /// <summary>
        /// Convert the stack of items into the corresponding inline elements in the textBlock.
        /// </summary>
        /// <param name="textBlock"></param>
        /// <param name="itemStack"></param>
        private static void GenerateFormattedText(perFormattedTextBlock textBlock, Stack<perFormattedTextItem> itemStack)
        {
            while (itemStack.Any())
            {
                var currentItem = itemStack.Pop();

                if (currentItem.IsLineBreak)
                    textBlock.Inlines.Add(new LineBreak());
                else
                {
                    var run = new Run(currentItem.Content)
                    {
                        FontSize = currentItem.FontSize,
                        FontFamily = currentItem.FontFamily,
                        Foreground = currentItem.Foreground,
                        Background = currentItem.Background,
                        FontWeight = currentItem.IsBold ? FontWeights.Bold : FontWeights.Normal,
                        FontStyle = currentItem.IsItalic ? FontStyles.Italic : FontStyles.Normal,
                        BaselineAlignment = currentItem.BaselineAlignment
                    };

                    if (currentItem.IsUnderline)
                    {
                        // What brush should the underline be drawn with?
                        var underlineDecoration = new TextDecoration
                        {
                            Pen = new Pen
                            {
                                Brush = textBlock.UnderlineUsesCurrentForegroundBrush
                                    ? currentItem.Foreground
                                    : textBlock.Foreground
                            },
                            PenThicknessUnit = TextDecorationUnit.FontRecommended
                        };

                        var underline = new Underline(run)
                        {
                            TextDecorations = new TextDecorationCollection { underlineDecoration }
                        };

                        textBlock.Inlines.Add(underline);
                    }
                    else
                        textBlock.Inlines.Add(run);
                }
            }
        }

        // ================================================================================

        /// <summary>
        /// Data for a run within a formatted text block.
        /// </summary>
        private class perFormattedTextItem
        {
            public perFormattedTextItem()
            {
            }

            public perFormattedTextItem(string text, bool isBold, bool isUnderline, bool isItalic, double fontSize, FontFamily fontFamily, Brush foreground, Brush background, BaselineAlignment baselineAlignment)
            {
                Content = text;
                IsBold = isBold;
                IsUnderline = isUnderline;
                IsItalic = isItalic;
                FontSize = fontSize;
                FontFamily = fontFamily;
                Foreground = foreground;
                Background = background;
                BaselineAlignment = baselineAlignment;
            }

            public string Content { get; }
            public bool IsBold { get; }
            public bool IsUnderline { get; }
            public bool IsItalic { get; }
            public bool IsLineBreak { get; set; }
            public double FontSize { get; }
            public FontFamily FontFamily { get; }
            public Brush Foreground { get; }
            public Brush Background { get; }
            public BaselineAlignment BaselineAlignment { get; }
        }

        // ================================================================================

        private enum TagType
        {
            None,
            Bold,
            Italic,
            Underline,
            FontSize,
            FontFamily,
            Foreground,
            Background,
            Subscript,
            SuperScript,
            LineBreak,
            LessThan,
            GreaterThan,
            Slash
        }

        // ================================================================================

        /// <summary>
        /// The tag definitions for each element of TagType
        /// </summary>
        private static class TagTypeConstants
        {
            private static readonly Dictionary<TagType, string> OpeningTags;
            private static readonly Dictionary<TagType, string> ClosingTags;
            private static readonly Dictionary<TagType, string> Tags;

            static TagTypeConstants()
            {
                OpeningTags = new Dictionary<TagType, string>();
                ClosingTags = new Dictionary<TagType, string>();
                Tags = new Dictionary<TagType, string>();

                OpeningTags[TagType.Bold] = "<b>";
                ClosingTags[TagType.Bold] = "</b>";

                OpeningTags[TagType.Italic] = "<i>";
                ClosingTags[TagType.Italic] = "</i>";

                OpeningTags[TagType.Underline] = "<u>";
                ClosingTags[TagType.Underline] = "</u>";

                OpeningTags[TagType.LineBreak] = "<lb>";
                OpeningTags[TagType.LessThan] = "<lt>";
                OpeningTags[TagType.GreaterThan] = "<gt>";
                OpeningTags[TagType.Slash] = "<sl>";

                OpeningTags[TagType.Subscript] = "<sub>";
                ClosingTags[TagType.Subscript] = "</sub>";

                OpeningTags[TagType.SuperScript] = "<sup>";
                ClosingTags[TagType.SuperScript] = "</sup>";

                // regex expression elements :-
                // Font Size
                //     <fs[ =]?(?:[0-9]+|\*[0-9]+(?:\.[0-9]+)?)>
                //
                //     <fs                                     >        literal characters, the start and end of the tag
                //        [ =]?                                         optionally, either exactly one space or one literal = character
                //             (?:                            )         a mandatory non-capturing group, consisting of either  
                //                [0-9]+                                    at least one numeric digit
                //                      |                               or
                //                       \*[0-9]+                           a literal * character followed by at least 1 numeric digit
                //                               (?:        )?              optionally followed 1 time by a non-capturing group consisting of  
                //                                  \.[0-9]+                    a literal . character followed by at least 1 numeric digit
                //
                // The following will all match
                //     <fs9>
                //     <fs 9>
                //     <fs=9>
                //     <fs99>
                //     <fs 99>
                //     <fs=99>
                //     <fs*9.9>
                //     <fs *9.9>
                //     <fs=*9.9>
                //     <fs*99.9>
                //     <fs *99.9>
                //     <fs=*99.9>
                //     <fs*9.99>
                //     <fs *9.99>
                //     <fs=*9.99>
                //
                //
                // Font Family
                //     <ff[ =]?[a-z\s]+>
                //
                //     <ff             >        literal characters, the start and end of the tag   
                //        [ =]?                 optionally, either exactly one space or one literal = character
                //             [a-z\s]+         followed by a mix of one or more letters / spaces (font family names can contain space characters) 
                //    
                //
                // Foreground / Background
                //    <fg[ =]?(?:[#][a-f0-9]{6}|[#][a-f0-9]{8}|[a-z]+)>
                //
                //    <fg                                             >        literal characters, the start and end of the tag   
                //       [ =]?                                                 optionally, either exactly one space or one literal = character
                //            (?:                                    )         a mandatory non-capturing group, consisting of either 
                //               [#][a-f0-9]{6}                                    a literal # character followed by exactly 6 hex digits (#RRGGBB)
                //                             |                               or 
                //                              [#][a-f0-9]{8}                     a literal # character followed by exactly 8 hex digits (#AARRGGBB)
                //                                            |                or                   
                //                                             [a-z]+              one or more letters - wpf colour names only contain letters
                //
                // similarly for bg tag
                //
                // There's no need to worry about cases in the patterns as all regex matches are done case-insensitive.

                OpeningTags[TagType.FontSize] = @"<fs[ =]?(?:[0-9]+|\*[0-9]+(?:\.[0-9]+)?)>";
                ClosingTags[TagType.FontSize] = @"</fs>";
                Tags[TagType.FontSize] = "fs";

                OpeningTags[TagType.FontFamily] = @"<ff[ =]?[a-z\s]+>";
                ClosingTags[TagType.FontFamily] = @"</ff>";
                Tags[TagType.FontFamily] = "ff";

                OpeningTags[TagType.Foreground] = @"<fg[ =]?(?:[#][a-f0-9]{6}|[#][a-f0-9]{8}|[a-z]+)>";
                ClosingTags[TagType.Foreground] = @"</fg>";
                Tags[TagType.Foreground] = "fg";

                OpeningTags[TagType.Background] = @"<bg[ =]?(?:[#][a-f0-9]{6}|[#][a-f0-9]{8}|[a-z]+)>";
                ClosingTags[TagType.Background] = @"</bg>";
                Tags[TagType.Background] = "bg";
            }

            public static string GetOpeningTag(TagType tagType) => OpeningTags.ContainsKey(tagType) ? OpeningTags[tagType] : string.Empty;
            public static string GetClosingTag(TagType tagType) => ClosingTags.ContainsKey(tagType) ? ClosingTags[tagType] : string.Empty;
            public static string GetTag(TagType tagType) => Tags.ContainsKey(tagType) ? Tags[tagType] : string.Empty;
        }
    }
}