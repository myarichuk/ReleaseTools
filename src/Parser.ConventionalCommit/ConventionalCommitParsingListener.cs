using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;

namespace Parser.ConventionalCommit
{
    internal class ConventionalCommitParsingListener: ConventionalCommitParserBaseListener
    {
        private ConventionalCommit _parseResult = new();
        private static readonly ConcurrentQueue<StringBuilder> StringBuilderPool = new(); //this is simple use-case, no need for "fancy" pools

        public void Reset() => _parseResult = new();

        public ConventionalCommit ParsedMessage => _parseResult;

        public override void EnterCommitMessage(ConventionalCommitParser.CommitMessageContext context)
        {
            var typeAsString = context.type()?.GetText() ?? throw new InvalidDataException("Failed to parse commit type, it was null!");
            _parseResult.Type = !Enum.TryParse<CommitType>(typeAsString, true, out var typeAsEnum) ? 
                CommitType.Other : typeAsEnum;

            _parseResult.TypeAsString = typeAsString;

            if (context.scope != null)
            {
                _parseResult.Scope =
                    context.scope.Text.Trim(' ', '\t');
            }

            _parseResult.Description = 
                context.description?.Text.Trim(' ', '\t') ?? string.Empty;
        }

        public override void EnterBody(ConventionalCommitParser.BodyContext context)
        {
            if (!StringBuilderPool.TryDequeue(out var sb))
            {
                sb = new StringBuilder((int)(context.GetText().Length * 1.5));
            }

            try
            {
                foreach (var bodyLine in context.TEXT())
                {
                    sb.AppendLine(bodyLine?.GetText()
                                      .Trim(' ', '\t')
                                  ?? string.Empty);
                }
                _parseResult.Body = sb.ToString();

                if (_parseResult.Body.EndsWith(Environment.NewLine))
                {
                    _parseResult.Body = _parseResult.Body.Substring(0, _parseResult.Body.Length - Environment.NewLine.Length);
                }
            }
            finally
            {
                sb.Clear();
                StringBuilderPool.Enqueue(sb);
            }
        }

        public override void EnterFooter(ConventionalCommitParser.FooterContext context)
        {
            if (context.exception != null)
            {
                throw new InvalidOperationException("Failed to parse the footer section", context.exception); //failed to parse the footer
            }
            foreach (var tuple in context.footerTuple())
            {
                var key = tuple.key?.Text.Trim(' ', '\t');
                var value = tuple.value?.Text.Trim(' ', '\t');

                if (key == null || value == null)
                {
                    continue;
                }

                _parseResult.AddFooterItem(key, value);
            }
        }
    }
}
