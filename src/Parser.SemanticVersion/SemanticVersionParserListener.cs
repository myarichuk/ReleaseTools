using System;
using System.Collections.Generic;

namespace Parser.SemanticVersion
{
    internal class SemanticVersionParserListener: SemanticVersionParserBaseListener
    {
        
        private readonly Queue<string> _buildQueue = new();
        private readonly Queue<string> _preReleaseQueue = new();
        private int _minor;
        private int _major;
        private int _patch;

        private SemanticVersionTag _buildTag = new(SemanticTag.None, null, null);
        private SemanticVersionTag _preReleaseTag = new(SemanticTag.None, null, null);

        public SemanticVersion ParsedSemver => new()
        {
            Major = _major,
            Minor = _minor,
            Patch = _patch,
            Build = string.Join(string.Empty, _buildQueue),
            PreRelease = string.Join(string.Empty, _preReleaseQueue),
            BuildVersion = _buildTag,
            PreReleaseVersion = _preReleaseTag,
        };

        public void Reset()
        {
            _minor = _major = _patch = 0;
            _buildQueue.Clear();
            _preReleaseQueue.Clear();
            _buildTag = new(SemanticTag.None, null, null);
            _preReleaseTag = new(SemanticTag.None, null, null);
        }

        public override void EnterVersionCore(SemanticVersionParser.VersionCoreContext context)
        {
            _patch = int.Parse(context.patch.Text);
            _minor = int.Parse(context.minor.Text);
            _major = int.Parse(context.major.Text);
        }

        public override void EnterBuildNumber(SemanticVersionParser.BuildNumberContext context) => 
            _buildQueue.Enqueue(context.value.Text);

        public override void EnterBuildIdentifier(SemanticVersionParser.BuildIdentifierContext context) => 
            _buildQueue.Enqueue(context.value.Text);

        public override void EnterBuildTagged(SemanticVersionParser.BuildTaggedContext context)
        {
            _buildQueue.Enqueue(context.tag().GetText());

            if (context.version != null)
            {
                _buildQueue.Enqueue(context.version.Text);
            }

            if (!Enum.TryParse<SemanticTag>(context.tag().GetText(), true, out var tagAsEnum))
            {
                return;
            }

            //just in case
            _buildTag.Tag = tagAsEnum;

            if (context.version == null)
            {
                return;
            }

            if (int.TryParse(context.version.Text, out var versionAsInt))
            {
                _buildTag.Version = versionAsInt;
            }
            else
            {
                _buildTag.Identifier = context.version.Text;
            }
        }

        public override void EnterPreReleaseIdentifier(SemanticVersionParser.PreReleaseIdentifierContext context) => 
            _preReleaseQueue.Enqueue(context.value.Text);

        public override void EnterPreReleaseTagged(SemanticVersionParser.PreReleaseTaggedContext context)
        {
            _preReleaseQueue.Enqueue(context.tag().GetText());

            if (context.version != null)
            {
                _preReleaseQueue.Enqueue(context.version.Text);
            }

            if (!Enum.TryParse<SemanticTag>(context.tag().GetText(), true, out var tagAsEnum))
            {
                return;
            }

            //just in case
            _preReleaseTag.Tag = tagAsEnum;

            if (context.version == null)
            {
                return;
            }

            if (int.TryParse(context.version.Text, out var versionAsInt))
            {
                _preReleaseTag.Version = versionAsInt;
            }
            else
            {
                _preReleaseTag.Identifier = context.version.Text;
            }
        }

        public override void EnterPreReleaseNumber(SemanticVersionParser.PreReleaseNumberContext context) => 
            _preReleaseQueue.Enqueue(context.value.Text);
    }
}
