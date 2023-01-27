using System;
using System.Collections.Generic;
using System.Text;

namespace ConventionalCommitsParser
{
    internal class ConventionalCommitParsingListener: ConventionalCommitBaseListener
    {
        private readonly ConventionalCommit _parseResult = new();

        public ConventionalCommit ParsedMessage => _parseResult;

        public override void EnterDescription(ConventionalCommitParser.DescriptionContext context) => 
            _parseResult.Description = context.value.Text;

        public override void EnterCommitType(ConventionalCommitParser.CommitTypeContext context)
        {
            var parsedType = Enum.TryParse<CommitType>(context.value.Text, true, out var parsedEnum) ? parsedEnum : CommitType.Other;
            _parseResult.Type = parsedType;
        }

        public override void EnterCommitScope(ConventionalCommitParser.CommitScopeContext context) => _parseResult.Scope = context.value.Text;

        public override void EnterBodyLine(ConventionalCommitParser.BodyLineContext context)
        {
            _parseResult.Body ??= string.Empty;

            _parseResult.Body += context.value.Text;
        }

        public override void EnterFooterItem(ConventionalCommitParser.FooterItemContext context)
        {
            var key = context.footerKey().value.Text;
            var value = context.footerValue().value.Text;

            _parseResult.AddFooterItem(key, value);
        }
    }
}
