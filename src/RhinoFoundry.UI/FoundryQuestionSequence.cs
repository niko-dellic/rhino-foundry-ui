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
    /// <summary>Raised asynchronously when the user closes the sequence; nothing is submitted.</summary>
    public event EventHandler? Cancelled;
    public IReadOnlyList<string> Answers => _answers.ToArray();

    public FoundryQuestionSequence(IReadOnlyList<FoundryQuestion> questions)
    {
        if (questions.Count == 0) throw new ArgumentException("Provide at least one question.", nameof(questions));
        _questions = questions.ToArray();
        _answers = new string[questions.Count];
        _customDrafts = new string[questions.Count];
        Content = _body;
        Padding = new Eto.Drawing.Padding(FoundryTheme.Space3);
        BackgroundColor = FoundryTheme.PanelBackground;
        KeyDown += (_, e) => { if (e.Key == Keys.Escape) { e.Handled = true; Cancel(); } };
        Render();
    }

    private void Cancel()
    {
        if (_submitted || IsDisposed) return;
        _submitted = true;
        Application.Instance.AsyncInvoke(() => { if (!IsDisposed) Cancelled?.Invoke(this, EventArgs.Empty); });
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
        counter.VerticalAlignment = VerticalAlignment.Center;
        var counterHost = new Panel { Height = 32, Content = counter };
        var close = new FoundryToolbarIconButton(FoundryViewIcons.Close(), "Cancel questions without submitting");
        close.Click += (_, _) => Cancel();
        var title = new Label { Text = question.Text, Wrap = WrapMode.Word, TextAlignment = TextAlignment.Left, TextColor = FoundryTheme.PrimaryText };
        title.LoadComplete += (_, _) => { title.TextAlignment = TextAlignment.Right; title.TextAlignment = TextAlignment.Left; };
        _body.Items.Add(new StackLayout { Orientation = Orientation.Horizontal, Spacing = FoundryTheme.Space2,
            VerticalContentAlignment = VerticalAlignment.Center,
            Items = { new StackLayoutItem(title, true), previous, counterHost, next, close } });
        for (var optionIndex = 0; optionIndex < question.Options.Count; optionIndex++)
        {
            var answer = question.Options[optionIndex];
            var description = question.Descriptions is { } descriptions && optionIndex < descriptions.Count ? descriptions[optionIndex] : "";
            var option = new FoundryQuestionAnswer(optionIndex + 1, answer, description);
            option.Click += (_, _) =>
            {
                Answer(pageIndex, answer);
            };
            _body.Items.Add(option);
        }
        var editor = new TextArea { Text = _customDrafts[pageIndex] ?? "", Wrap = true, ToolTip = "Say something else…" };
        editor.TextChanged += (_, _) => _customDrafts[pageIndex] = editor.Text;
        var pencil = new FoundryToolbarIconButton(FoundryViewIcons.Pencil(), "Write a custom answer");
        pencil.Click += (_, _) => editor.Focus();
        var skip = new FoundryDialogButton("Skip", FoundryDialogButtonStyle.Secondary, 60) { ToolTip = "Leave this question unanswered and continue" };
        skip.Click += (_, _) => Answer(pageIndex, "[Skipped — no answer supplied]");
        _body.Items.Add(new StackLayout { Orientation = Orientation.Horizontal, Spacing = FoundryTheme.Space2,
            VerticalContentAlignment = VerticalAlignment.Center,
            Items = { pencil, new StackLayoutItem(new FoundryGrowingTextField(editor, 120, placeholder: "Say something else..."), true), skip } });
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

public sealed record FoundryQuestion(string Text, IReadOnlyList<string> Options)
{
    public IReadOnlyList<string>? Descriptions { get; init; }
}
