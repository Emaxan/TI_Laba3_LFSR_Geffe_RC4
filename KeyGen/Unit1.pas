unit Unit1;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls;

type
  TForm1 = class(TForm)
    edt1: TEdit;
    btn1: TButton;
    edt2: TEdit;
    lbl1: TLabel;
    lbl2: TLabel;
    procedure btn1Click(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  Form1: TForm1;

implementation

{$R *.dfm}

function Work(deg: Integer; Count: Integer): Integer;
var
  i, j, max: integer;
  s: set of Byte;
begin
  Randomize;
  max := 1;
  for i := 1 to deg do
    max := max * 2;
  AssignFile(Output, '..\Key.txt');
  Rewrite(Output);
  s := [];
  for j := 1 to Count do
  begin
    if (Count <= max) then
      repeat
        i := Random(Max);
      until not (i in s)
    else
    begin
      Result := 1;
      Exit;
    end;
    write(output, i, ' ');
    s := s + [i];
  end;
  CloseFile(Output);
  Result := 0;
end;

procedure TForm1.btn1Click(Sender: TObject);
begin
  if (Work(StrToInt(edt1.Text), StrToInt(edt2.Text)) <> 0) then
    edt1.Text := 'Wrong Count';
end;

end.

