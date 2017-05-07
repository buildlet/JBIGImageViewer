BUILDLet JBIG Image Viewer
==========================

Version 2.2.1.0
---------------

概要
----

JBIG1 フォーマットの画像ファイルを閲覧するソフトウェアです。

JBIG Image Viewer は、次の方法で JBIG1 フォーマットの画像を変換・表示します。

  1. jbgtopbm.exe (JbigKit for Windows) を実行して、JIBG1 フォーマットの画像ファイルを 
     PBM (Portable Bitmap) / PPM (Portable Pixel Map) フォーマットに変換して、一時ファイルとして出力します。
  2. ppmtobmp.exe (NetPbm for Windows) を実行して、一時ファイルをビットマップ画像ファイルに変換して表示します。

Remove unexpected data オプションを有効にすることによって、JBIG データの中に、ある特定のパターンで始まる
テキスト行が出現した場合に、そのテキスト行を除去します。
また、テキスト行のライン数がある閾値に達すると、それ以降を次のページとして扱います。

JBIG イメージファイルをビットマップ イメージファイルとして保存する場合は、一時ファイルとして作成した
ビットマップ イメージファイルをコピーします。  
その他のイメージ フォーマットで保存する場合は、一時ファイルとして作成した PBM イメージ ファイルから、
変換先のフォーマットのファイルを作成します。


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

このプログラムを実行するためには、Microsoft .NET Framework 4.5 がインストールされている必要があります。

Windows 10 Pro (1607) x64 および Windows 7 Ultimate Service Pack 1x64 で動作を確認しています。


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

* Version 2.2.1.0 (2017/05/07)  
  全てのモジュールを刷新しました。  
  ファイルの保存形式として、PNG, JPEG, GIF および TIFF をサポートし、デフォルトを PNG としました。  
  BUILDLet JBIG Image Converter Library (BUILDLet.JBIG) の Help Documentation (API Reference) を追加しました。

* Version 1.2.2.0 (2015/09/30)  
  マイナーアップデート  
  デスクトップ ショートカットのファイル名を変更しました。

* Version 1.2.1.0 (2015/09/29)  
  拡大縮小の原点を変更。マウスホイールの操作によって画像を拡大・縮小する際は、マウスカーソルの位置を原点に、
  キー操作によって画像を拡大・縮小する際は、画面中央を原点に、それぞれ画像の拡大縮小を行うようように処理を変更しました。  
  拡大縮小率を調整しました。

* Version 1.2.0.0 (2015/09/28)  
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

* Version 1.1.1.0 (2015/08/13)  
  マイナーアップデート  
  デスクトップ ショートカットのファイル名を変更しました。

* Version 1.1.0.0 (2015/07/26)  
  テキスト除去とページ分割処理を見直しました。  
  ビットマップに変換済みのファイルは、ファイルを閉じるまで保持し、
  ファイルを閉じるタイミングで削除するように処理を見直しました。  
  アイコン画像の微修正。

* Version 1.0.0.0 (2015/07/05)  
  初版
