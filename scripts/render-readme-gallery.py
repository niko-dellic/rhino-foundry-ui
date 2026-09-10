#!/usr/bin/env python3
"""Generate schematic SVG documentation, not native Eto screenshots. No dependencies."""
from pathlib import Path
from html import escape

OUT = Path(__file__).resolve().parents[1] / 'docs' / 'images'
# Fixed light illustration palette corresponding to FoundryTheme roles. Native
# SystemColors and Rhino selection preferences are deliberately not simulated.
T = dict(panel='#fafafa', surface='#ffffff', subtle='#f4f4f5', border='#d4d4d8',
         text='#27272a', secondary='#52525b', muted='#71717a', active='#e4e4e7')
parts = []
def rect(x,y,w,h,fill='surface',radius=6,stroke='border'):
    parts.append(f'<rect x="{x}" y="{y}" width="{w}" height="{h}" rx="{radius}" fill="{T[fill]}" stroke="{T[stroke]}"/>')
def text(x,y,s,size=14,color='text',bold=False):
    parts.append(f'<text x="{x}" y="{y}" font-size="{size}" fill="{T[color]}" font-weight="{600 if bold else 400}">{escape(s)}</text>')
def line(x,y,x2,y2):
    parts.append(f'<path d="M{x} {y}H{x2}" stroke="{T["border"]}"/>' if y==y2 else f'<path d="M{x} {y}L{x2} {y2}" stroke="{T["border"]}"/>')
def button(x,y,s,w=100,fill='panel',height=32):
    rect(x,y,w,height,fill); text(x+12,y+21,s,13)
def label(x,y,s): text(x,y,s,12,'secondary',True)
def start(title,desc):
    parts.clear()
    parts.append('<svg xmlns="http://www.w3.org/2000/svg" width="960" height="440" viewBox="0 0 960 440" role="img" aria-labelledby="title desc">')
    parts.append(f'<title id="title">{escape(title)}</title><desc id="desc">{escape(desc)}</desc>')
    parts.append('<g font-family="-apple-system, BlinkMacSystemFont, Segoe UI, sans-serif">')
    rect(0.5,0.5,959,439,'panel',12)
    text(28,38,title,22,bold=True); text(28,62,'COMPONENT ILLUSTRATIONS · LIGHT PALETTE · NOT NATIVE SCREENSHOTS',10,'muted')
def save(name):
    parts.append('</g></svg>'); (OUT / f'{name}.svg').write_text('\n'.join(parts)+'\n')

start('Actions & input', 'Buttons, fields, checkboxes, slider, color input, editable title, growing text and 34 pixel compatibility variants.')
label(28,100,'Dialog buttons · action rows')
button(28,114,'Continue'); button(136,114,'Cancel'); button(244,114,'Disabled',100,'subtle')
label(28,180,'FormField · ToolbarField · SearchField')
rect(28,194,316,32); text(40,215,'Name or value',13,'secondary')
rect(28,238,316,32); text(40,259,'⌕  Search components…',13,'muted')
label(28,304,'CheckBox · ColorField · Slider')
rect(28,320,18,18,'active',4); text(31,334,'✓',13); text(56,334,'Enabled',13)
rect(188,316,156,32); rect(197,323,18,18,'secondary',3); text(225,338,'Choose color',12)
line(28,379,344,379); rect(176,371,16,16,'surface',8)
label(396,100,'EditableTitle')
text(396,138,'Untitled conversation',18,bold=True); text(648,138,'✎',18,'secondary')
label(396,180,'GrowingTextField')
rect(396,194,532,76); text(408,217,'A compact editor that grows with your text.',14)
text(408,241,'Height stays bounded as the content expands.',14,'secondary')
label(396,304,'SurfaceButton · InsetFormField · SurfaceTheme')
button(396,319,'34px family',130,height=34); rect(538,319,390,34); text(550,341,'Inset field',13,'secondary')
text(396,389,'Standard controls: 32px · compatibility variants: 34px',12,'secondary')
save('actions-input')

start('Choice & layout', 'Toolbar groups and view modes, text segments, single and multi pickers, removable badges, accordion and a resize handle inside a scroll container.')
label(28,100,'ToolbarIconButton · ButtonGroup · ViewModeSelector')
for x,s in [(28,'☷'),(64,'▦'),(100,'□')]: button(x,114,s,32,'active' if x==64 else 'panel')
line(146,120,146,140); button(160,114,'+',32); button(200,114,'…',32)
label(28,181,'TextSegmentedControl')
rect(28,194,296,32,'subtle'); button(31,194,'First',94,'active'); text(143,215,'Second',13); text(242,215,'Third',13)
label(28,267,'FilteredPicker · MultiSelectField · RemovableBadge')
rect(28,281,380,32); text(40,302,'Select an item',13); text(384,302,'⌄',14)
rect(28,325,380,48); button(36,333,'Alpha ×',94,'subtle'); button(138,333,'Beta ×',88,'subtle'); text(384,354,'⌄',14)
label(472,100,'Accordion · Item · Trigger · Scrollable · PaneResizeHandle')
rect(472,114,456,278); text(488,141,'⌄  Expanded section',14,bold=True); line(488,155,904,155)
text(488,184,'Consumer-supplied content',14,'secondary'); rect(488,202,394,32); text(500,223,'Field inside a section',13,'muted')
line(488,257,904,257); text(488,286,'›  Collapsed section',14,bold=True)
rect(914,133,4,168,'active',2); line(896,163,896,243); text(488,354,'Sections, scrolling and pane resizing',13,'secondary')
save('choice-layout')

start('Tables, galleries & canvas', 'Flat and hierarchical grids with badge cells, a read-only table with row actions, thumbnail choices and a pan and zoom canvas foundation.')
label(28,100,'GridView · TreeGridView · Table · BadgeCell<T>')
rect(28,114,420,118); text(42,137,'Name',12,bold=True); text(310,137,'Status',12,bold=True); line(28,147,448,147)
text(42,170,'⌄  Collection',13); rect(304,154,122,24,'subtle'); text(316,171,'3 items',12)
text(60,204,'Item A',13); rect(304,188,122,24,'subtle'); text(316,205,'Ready',12)
label(28,271,'ReadOnlyTable · optional row actions')
rect(28,285,420,110); text(42,308,'Item',12,bold=True); text(181,308,'Description',12,bold=True); line(28,318,448,318)
text(42,347,'Alpha',13); text(181,347,'Wrapping text',13); text(181,367,'without nested scroll',13); button(362,335,'Open',70)
label(484,100,'ThumbnailGallery')
for x,s in [(484,'Alpha'),(636,'Beta'),(788,'Gamma')]:
    rect(x,114,140,118,'active' if s=='Beta' else 'surface'); rect(x+12,124,116,74,'subtle'); text(x+42,168,'▧',27,'secondary'); text(x+12,220,s,13)
label(484,271,'Canvas · camera, pan & zoom foundations')
rect(484,285,444,110,'subtle')
for x in range(500,920,20):
    for y in range(300,385,20): parts.append(f'<circle cx="{x}" cy="{y}" r="0.8" fill="{T["muted"]}"/>')
rect(552,304,168,68); text(565,330,'Your renderer',14,bold=True); text(565,352,'Your world units',12,'secondary')
text(748,345,'↔  Pan / zoom',13)
save('data-canvas')

start('Conversation & review', 'Chat and Markdown messages, a composer, question sequence, activity timeline and cards, resource chip, preflight facts and approval review surface.')
label(28,100,'ChatMessage · MarkdownMessage')
rect(124,114,324,40,'subtle',16); text(140,139,'Help me review these items.',14)
text(28,182,'Review ready',18,bold=True); text(28,208,'Selectable Markdown with lists, code and tables.',13,'secondary')
label(28,246,'ChatComposer · GrowingTextField')
rect(28,260,420,72,'surface',16); text(42,285,'Write a message…',13,'muted'); button(358,292,'Send',76)
label(28,367,'ResourceChip')
button(28,380,'File · reference.pdf',240)
label(484,100,'QuestionSequence')
text(484,127,'How should the review proceed?',15,bold=True)
button(484,140,'1',32); text(528,161,'Compare options',13,bold=True); button(690,140,'Recommended',132,'subtle')
label(484,201,'ActivityTimeline · ActivityCard')
text(484,226,'✓  Review · Finished',13,'secondary'); text(484,247,'Three items are ready to inspect.',13)
label(484,286,'ApprovalCard · PreflightSummary')
rect(484,300,444,112,'panel',0); text(498,322,'Review request',13,bold=True)
button(498,331,'Alpha',74,'subtle'); button(580,331,'Beta',70,'subtle'); text(498,390,'Scope',12,'secondary'); text(565,390,'Selected items',13)
save('conversation-review')
