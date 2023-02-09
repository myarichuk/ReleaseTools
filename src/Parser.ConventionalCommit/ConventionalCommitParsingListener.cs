using System;
using System.Linq;

namespace Parser.ConventionalCommit
{
    internal class ConventionalCommitParsingListener: ConventionalCommitParserBaseListener
    {
        private ConventionalCommit _parseResult = new();

        public void Reset() => _parseResult = new();

        public ConventionalCommit ParsedMessage => _parseResult;

        public override void EnterCommitMessage(ConventionalCommitParser.CommitMessageContext context)
        {
        }

        public override void EnterBody(ConventionalCommitParser.BodyContext context)
        {
        }

        public override void EnterFooter(ConventionalCommitParser.FooterContext context)
        {

        }

        //public override void EnterDescription(ConventionalCommitParser.DescriptionContext context) => 
        //    _parseResult.Description = context.value.Text.Trim(' ', '\t', '\r');

        //public override void EnterCommitType(ConventionalCommitParser.CommitTypeContext context)
        //{
        //    var parsedType = context.value != null && Enum.TryParse<CommitType>(context.value.Text, true, out var parsedEnum) ? 
        //        parsedEnum : CommitType.Other;

        //    _parseResult.Type = parsedType;
        //    if (!_parseResult.IsBreaking)
        //    {
        //        _parseResult.IsBreaking = context.isBreaking != null;
        //    }
        //}

        //public override void EnterCommitScope(ConventionalCommitParser.CommitScopeContext context)
        //{
        //    _parseResult.Scope = context.value?.Text.Trim();
        //    if (!_parseResult.IsBreaking)
        //    {
        //        _parseResult.IsBreaking = context.isBreaking != null;
        //    }
        //}

        //public override void EnterBody(ConventionalCommitParser.BodyContext context) => 
        //    _parseResult.Body = string.Join(Environment.NewLine, 
        //        context.bodyLine().Select(x => 
        //            (x.value?.Text ?? string.Empty).Trim(' ', '\n','\r')));

        //public override void EnterFooterItem(ConventionalCommitParser.FooterItemContext context)
        //{
        //    var key = context.footerKey().value?.Text.Trim(' ', '\n','\r');
        //    var value = context.footerValue().value?.Text.Trim(' ', '\n','\r');

        //    var keyContext = context.footerKey();
        //    if (keyContext.BREAKING_CHANGE() != null && !_parseResult.IsBreaking)
        //    {
        //        _parseResult.IsBreaking = true;
        //    }

        //    if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
        //    {
        //        _parseResult.AddFooterItem(key!, value!);
        //    }
        //}
    }
}
