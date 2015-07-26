JbigViewer
==========

JBIG Image Viewer Version **1.1.0.0**


概要
----
JBIG1 フォーマットの画像ファイルを閲覧するソフトウェアです。  

![Screenshot](/images/JBIGView.png "Screenshot")

JBIG Image Viewer は、次の方法で JBIG1 フォーマットの画像を変換・表示します。  
  1. jbgtopbm.exe (JbigKit for Windows) を実行して、JIBG1 フォーマットの画像ファイルを 
     PBM (Portable Bitmap) / PPM (Portable Pixel Map) フォーマットに変換して、一時ファイルとして出力します。
  2. ppmtobmp.exe (NetPbm for Windows) を実行して、一時ファイルをビットマップ画像ファイルに変換して表示します。

JBIG データの中に、ある特定のパターンで始まるテキスト行が出現した場合、そのテキスト行を除去します。  
テキスト行のライン数がある閾値に達すると、それ以降を次のページとして扱います。


インストール方法
----------------
JBIGViewSetup.exe を実行してください。


アンインストール方法
--------------------
コントロールパネルから BUILDLet JBIG Image Viewer を選択してアンインストールを実行してください。  
  
%LOCALAPPDATA%\BUILDLet (C:\Users\<ユーザー名>\AppData\Local\BUILDLet) に "JBIGView.exe_Url_*"
という名前のフォルダーが残っている場合は、そのフォルダーごと削除してください。


動作環境
--------
このプログラムを実行するためには、Microsoft .NET Framework 4.5 が必要です。  
  
Windows 7 Ultimate x64 で動作を確認しています。


ライセンス
----------
このソフトウェアは MTI ライセンスの下で配布されます。  
[LICENCE](/LICENSE "LICENSE") を参照してください。


JbigKit for Windows および JBIG-KIT は GNU GPL の下で配布されます。  

JbigKit for Windows のソースコードは下記の URL からダウンロードしてください。  
[http://gnuwin32.sourceforge.net/packages/jbigkit.htm](http://gnuwin32.sourceforge.net/packages/jbigkit.htm "JbigKit for Windows")
  
  
NetPbm for Windows および Netpbm のライセンスについては、下記 URL の Legal Usability を参照してください。  
[http://netpbm.sourceforge.net/](http://netpbm.sourceforge.net/ "Netpbm")  

NetPbm for Windows のソースコードは下記の URL からダウンロードしてください。  
[http://gnuwin32.sourceforge.net/packages/netpbm.htm](http://gnuwin32.sourceforge.net/packages/netpbm.htm "NetPbm for Windows")


変更履歴
--------
### Version 1.1.0.0
**2015/07/26**  
テキスト除去とページ分割処理を見直し。  
ビットマップに変換済みのファイルは、ファイルを閉じるまで保持し、ファイルを閉じるタイミングで削除するように処理を見直し。  
アイコン画像を微修正。  

### Version 1.0.0.0
**2015/07/05**  
1st リリース
