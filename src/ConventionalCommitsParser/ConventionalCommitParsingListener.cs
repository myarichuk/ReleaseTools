﻿using System;
using System.Linq;

namespace ConventionalCommitsParser
{
    internal class ConventionalCommitParsingListener: ConventionalCommitBaseListener
    {
        private readonly ConventionalCommit _parseResult = new();

        public ConventionalCommit ParsedMessage => _parseResult;

        public override void EnterDescription(ConventionalCommitParser.DescriptionContext context) => 
            _parseResult.Description = context.value.Text.Trim(' ', '\t', '\r');

        public override void EnterCommitType(ConventionalCommitParser.CommitTypeContext context)
        {
            var parsedType = context.value != null && Enum.TryParse<CommitType>(context.value.Text, true, out var parsedEnum) ? 
                parsedEnum : CommitType.Other;

            _parseResult.Type = parsedType;
            if (!_parseResult.IsBreaking)
            {
                _parseResult.IsBreaking = context.isBreaking != null;
            }
        }

        public override void EnterCommitScope(ConventionalCommitParser.CommitScopeContext context)
        {
            _parseResult.Scope = context.value?.Text.Trim();
            if (!_parseResult.IsBreaking)
            {
                _parseResult.IsBreaking = context.isBreaking != null;
            }
        }

        public override void EnterBody(ConventionalCommitParser.BodyContext context) => 
            _parseResult.Body = string.Join(Environment.NewLine, context.bodyLine().Select(x => (x.value?.Text ?? string.Empty).Trim(' ', '\n','\r')));

        public override void EnterFooterItem(ConventionalCommitParser.FooterItemContext context)
        {
            var key = context.footerKey().value?.Text.Trim();
            var value = context.footerValue().value?.Text.Trim();

            _parseResult.AddFooterItem(key, value);
        }
    }
}
