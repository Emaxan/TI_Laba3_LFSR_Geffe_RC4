object Form1: TForm1
  Left = 404
  Top = 152
  Width = 258
  Height = 143
  Caption = 'Form1'
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  PixelsPerInch = 96
  TextHeight = 13
  object lbl1: TLabel
    Left = 8
    Top = 16
    Width = 33
    Height = 13
    Caption = 'Count:'
  end
  object lbl2: TLabel
    Left = 8
    Top = 48
    Width = 52
    Height = 13
    Caption = 'Max(2^n):'
  end
  object edt1: TEdit
    Left = 64
    Top = 40
    Width = 169
    Height = 21
    TabOrder = 0
  end
  object btn1: TButton
    Left = 8
    Top = 72
    Width = 225
    Height = 25
    Caption = 'Generate'
    TabOrder = 1
    OnClick = btn1Click
  end
  object edt2: TEdit
    Left = 64
    Top = 8
    Width = 169
    Height = 21
    TabOrder = 2
  end
end
