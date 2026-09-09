using Eto.Forms;

namespace RhinoFoundry.UI;

/// <summary>A composer replacement. Choosing an answer advances; the last complete answer queues Submitted.</summary>
public sealed class FoundryQuestionSequence : Panel
{
    private readonly IReadOnlyList<FoundryQuestion> _questions;
    private readonly string[] _answers;
    private readonly string[] _customDrafts;
    private readonly StackLayout _body = new() { HorizontalContentAlignment = HorizontalAlignment.Stretch, Spacing = FoundryTheme.Space2 };
    private int _index;
    private bool _renderQueued;
    private bool _submitted;
    public event EventHandler? Submitted;
    public IReadOnlyList<string> Answers => _answers.ToArray();

    public FoundryQuestionSequence(IReadOnlyList<FoundryQuestion> questions)
    {
        if (questions.Count == 0) throw new ArgumentException("Provide at least one question.", nameof(questions));
        _questions = questions.ToArray();
        _answers = new string[questions.Count];
        _customDrafts = new string[questions.Count];
        Content = _body;
        Render();
    }

    private void Render()
    {
        // Native input callbacks must finish before any clicked control is retired.
        if (_renderQueued || IsDisposed) return;
        _renderQueued = true;
        Application.Instance.AsyncInvoke(() =>
        {
            _renderQueued = false;
            if (!IsDisposed) RenderPage();
        });
    }

    private void Answer(int pageIndex, string answer)
    {
        if (_renderQueued || _submitted || IsDisposed || string.IsNullOrWhiteSpace(answer)) return;
        _answers[pageIndex] = answer.Trim();
        if (pageIndex < _questions.Count - 1)
        {
            _index = pageIndex + 1;
            Render();
            return;
        }
        var missing = Array.FindIndex(_answers, string.IsNullOrWhiteSpace);
        if (missing >= 0)
        {
            _index = missing;
            Render();
            return;
        }
        _submitted = true;
        // The native mouse/key event must unwind before the consumer replaces us.
        Application.Instance.AsyncInvoke(() =>
        {
            if (IsDisposed) return;
            Enabled = false;
            Submitted?.Invoke(this, EventArgs.Empty);
        });
    }

    private void RenderPage()
    {
        var old = _body.Items.Select(i => i.Control).Where(c => c is not null).ToArray();
        _body.Items.Clear();
        foreach (var control in old) control!.Dispose();
        var pageIndex = _index;
        var question = _questions[pageIndex];
        var previous = new FoundryDialogButton("←", FoundryDialogButtonStyle.Secondary, 32) { Enabled = _index > 0, ToolTip = "Previous question" };
        var next = new FoundryDialogButton("→", FoundryDialogButtonStyle.Secondary, 32) { Enabled = _index < _questions.Count - 1, ToolTip = "Next question" };
        previous.Click += (_, _) => { if (_renderQueued) return; _index = Math.Max(0, pageIndex - 1); Render(); };
        next.Click += (_, _) => { if (_renderQueued) return; _index = Math.Min(_questions.Count - 1, pageIndex + 1); Render(); };
        var counter = FoundryTheme.MutedLabel($"{_index + 1} of {_questions.Count}");
        counter.TextAlignment = TextAlignment.Center;
        var counterHost = new Panel { Height = 32, Padding = new Eto.Drawing.Padding(0, 8), Content = counter };
        _body.Items.Add(new StackLayout { Orientation = Orientation.Horizontal, Spacing = FoundryTheme.Space2,
            Items = { new StackLayoutItem(null, true), previous, counterHost, next } });
        var title = new Label { Text = question.Text, Wrap = WrapMode.Word, TextAlignment = TextAlignment.Left, TextColor = FoundryTheme.PrimaryText };
        title.LoadComplete += (_, _) => { title.TextAlignment = TextAlignment.Right; title.TextAlignment = TextAlignment.Left; };
        _body.Items.Add(title);
        foreach (var answer in question.Options)
        {
            var option = new FoundryDialogButton(answer, FoundryDialogButtonStyle.Secondary) { ToolTip = answer, LeftAlignText = true };
            option.Click += (_, _) =>
            {
                Answer(pageIndex, answer);
            };
            _body.Items.Add(option);
        }
        var editor = new TextArea { Text = _customDrafts[pageIndex] ?? "", Wrap = true, ToolTip = "Say something else…" };
        editor.TextChanged += (_, _) => _customDrafts[pageIndex] = editor.Text;
        _body.Items.Add(new FoundryGrowingTextField(editor, 120, placeholder: "Say something else..."));
        {
            var submit = new FoundryDialogButton("Use custom answer", FoundryDialogButtonStyle.Secondary);
            void Update() => submit.Visible = !string.IsNullOrWhiteSpace(editor.Text);
            editor.TextChanged += (_, _) => Update();
            Update();
            submit.Click += (_, _) =>
            {
                Answer(pageIndex, editor.Text);
            };
            editor.KeyDown += (_, e) =>
            {
                if (e.Key != Keys.Enter || e.Modifiers != Keys.None || string.IsNullOrWhiteSpace(editor.Text)) return;
                e.Handled = true;
                Answer(pageIndex, editor.Text);
            };
            _body.Items.Add(submit);
        }
    }
}

public sealed record FoundryQuestion(string Text, IReadOnlyList<string> Options);
