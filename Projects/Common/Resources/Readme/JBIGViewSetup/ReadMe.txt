BUILDLet JBIG Image Viewer
==========================

概要
----

JBIG1 フォーマットの画像ファイルを閲覧するソフトウェアです。

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

このソフトウェアは MIT ライセンスの下で配布されます。
License.txt を参照してください。


JbigKit for Windows および JBIG-KIT は GNU GPL の下で配布されます。
jbigkit.txt を参照してください。
(このファイルは jbigkit-1.6 の jbig.c の先頭部分を抜き出したものです。)

JbigKit for Windows のソースコードは下記の URL からダウンロードしてください。
http://gnuwin32.sourceforge.net/packages/jbigkit.htm


NetPbm for Windows および Netpbm のライセンスについては、下記 URL の Legal Usability を参照してください。
http://netpbm.sourceforge.net/

また、netpbm.txt を参照してください。
(このファイルは netpbm-10.27 の copyright_summary をリネームしたものです。)

NetPbm for Windows のソースコードは下記の URL からダウンロードしてください。
http://gnuwin32.sourceforge.net/packages/netpbm.htm


変更履歴
--------
* 2015/09/29    Version 1.2.0.0
  コンテキストメニューを追加しました。
  [ESC] キーでアプリケーションを終了しない仕様に変更しました。
  マウスのドラッグ操作によって画像の座標を移動できるようにしました。
  [Ctrl] キー + マウスホイールの操作によって画像の拡大縮小をできるようにしました。

  [Ctrl] + [+] キーで画像の拡大、[Ctrl] + [-] キーで画像の縮小をできるようにしました。
  拡大縮小の原点は画面左上です。

  [Ctrl] + 右矢印 (→) キー、および [Ctrl] + 左矢印 (←) キーでページの移動をできるようにしました。
  ページ番号を表示しているテキストボックスにフォーカスがあるときは KeyUp イベントでページを移動します。
  それ以外のコントロールにフォーカスがある場合は KeyDown イベントでページを移動しますが、
  [+] / [-] キーの KeyDown イベントが拾えない場合があり、その場合はページ移動できません。

* 2015/08/13    Version 1.1.1.0    マイナーアップデート
  デスクトップ ショートカットのファイル名を変更。

* 2015/07/26    Version 1.1.0.0
  テキスト除去とページ分割処理を見直し。
  ビットマップに変換済みのファイルは、ファイルを閉じるまで保持し、ファイルを閉じるタイミングで削除するように処理を見直し。
  アイコン画像を微修正。

* 2015/07/05    Version 1.0.0.0    初版
