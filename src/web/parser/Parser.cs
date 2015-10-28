using System;
using System.Collections.Generic;

namespace rift.parser
{
    public class Parser
    {
        protected string _html;
        protected int _pos;
        protected bool _scriptBegin;
        const string endComment = "-->";
        const string endScript = "</script";

        public Parser(string html)
        {
            Reset(html);
        }

        public void Reset()
        {
            _pos = 0;
        }

        public void Reset(string html)
        {
            _html = html;
            _pos = 0;
        }

        public bool Eof
        {
            get { return (_pos >= _html.Length); }
        }

        public bool ParseNext(string name, out Tag tag)
        {
            tag = null;

            if (String.IsNullOrEmpty(name))
            {
                return false;
            }

            while (MoveToNextTag())
            {
                Move();

                char c = Peek();
                if (c == '!' && Peek(1) == '-' && Peek(2) == '-')
                {
                    _pos = _html.IndexOf(endComment, _pos, StringComparison.OrdinalIgnoreCase);
                    NormalizePosition();
                    Move(endComment.Length);
                }
                else if (c == '/')
                {
                    _pos = _html.IndexOf('>', _pos);
                    NormalizePosition();
                    Move();
                }
                else
                {
                    bool result = ParseTag(name, ref tag);

                    if (_scriptBegin)
                    {
                        _pos = _html.IndexOf(endScript, _pos,  StringComparison.OrdinalIgnoreCase);
                        NormalizePosition();
                        Move(endScript.Length);
                        SkipWhitespace();
                        if (Peek() == '>')
                        {
                            Move();
                        }
                    }

                    if (result)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected bool ParseTag(string name, ref Tag tag)
        {
            string s = ParseTagName();

            bool doctype = _scriptBegin = false;
            if (String.Compare(s, "!DOCTYPE", true) == 0)
            {
                doctype = true;
            }
            else if (String.Compare(s, "script", true) == 0)
            {
                _scriptBegin = true;
            }

            bool requested = false;
            if (name == "*" || String.Compare(s, name, true) == 0)
            {
                tag = new Tag {Name = s, Attributes = new Dictionary<string, string>()};
                requested = true;
            }

            SkipWhitespace();
            while (Peek() != '>')
            {
                if (Peek() == '/')
                {
                    if (requested)
                    {
                        tag.TrailingSlash = true;
                    }

                    Move();
                    SkipWhitespace();
                    _scriptBegin = false;
                }
                else
                {
                    s = (!doctype) ? ParseAttributeName() : ParseAttributeValue();
                    SkipWhitespace();
                    string value = String.Empty;
                    if (Peek() == '=')
                    {
                        Move();
                        SkipWhitespace();
                        value = ParseAttributeValue();
                        SkipWhitespace();
                    }
                    if (requested)
                    {
                        if (tag.Attributes.Keys.Contains(s))
                        {
                            tag.Attributes.Remove(s);
                        }
                        tag.Attributes.Add(s, value);
                    }
                }
            }

            Move();

            return requested;
        }

        protected string ParseTagName()
        {
            int start = _pos;
            while (!Eof && !Char.IsWhiteSpace(Peek()) && Peek() != '>')
            {
                Move();
            }

            return _html.Substring(start, _pos - start);
        }

        protected string ParseAttributeName()
        {
            int start = _pos;
            while (!Eof && !Char.IsWhiteSpace(Peek()) && Peek() != '>' && Peek() != '=')
            {
                Move();
            }

            return _html.Substring(start, _pos - start);
        }

        protected string ParseAttributeValue()
        {
            int start, end;
            char c = Peek();
            if (c == '"' || c == '\'')
            {
                Move();
                start = _pos;
                _pos = _html.IndexOfAny(new char[] { c, '\r', '\n' }, start);
                NormalizePosition();
                end = _pos;
                if (Peek() == c)
                {
                    Move();
                }
            }
            else
            {
                start = _pos;
                while (!Eof && !Char.IsWhiteSpace(c) && c != '>')
                {
                    Move();
                    c = Peek();
                }
                end = _pos;
            }
            return _html.Substring(start, end - start);
        }

        protected bool MoveToNextTag()
        {
            _pos = _html.IndexOf('<', _pos);
            NormalizePosition();
            return !Eof;
        }

        public char Peek()
        {
            return Peek(0);
        }

        public char Peek(int ahead)
        {
            int pos = (_pos + ahead);
            if (pos < _html.Length)
            {
                return _html[pos];
            }

            return (char)0;
        }

        protected void Move()
        {
            Move(1);
        }

        protected void Move(int ahead)
        {
            _pos = Math.Min(_pos + ahead, _html.Length);
        }

        protected void SkipWhitespace()
        {
            while (!Eof && Char.IsWhiteSpace(Peek()))
            {
                Move();
            }
        }

        protected void NormalizePosition()
        {
            if (_pos < 0)
            {
                _pos = _html.Length;
            }
        }
    }
}