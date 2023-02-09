using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
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

            _parseResult.Scope = context.scope.Text;
            _parseResult.Description = context.description?.Text ?? string.Empty;
        }

        public override void EnterOtherType(ConventionalCommitParser.OtherTypeContext context)
        {
        }

        public override void EnterBody(ConventionalCommitParser.BodyContext context)
        {
            if (!StringBuilderPool.TryDequeue(out var sb))
            {
                sb = new StringBuilder(128);
            }

            try
            {
                foreach (var bodyLine in context.TEXT())
                {
                    sb.AppendLine(bodyLine.GetText());
                }
                _parseResult.Body = sb.ToString();
            }
            finally
            {
                sb.Clear();
                StringBuilderPool.Enqueue(sb);
            }
        }

        public override void EnterFooter(ConventionalCommitParser.FooterContext context)
        {
            foreach (var tuple in context.footerTuple())
            {
                _parseResult.AddFooterItem(tuple.key.Text, tuple.value.Text);
            }
        }
    }
}
