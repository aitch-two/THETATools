THETATools 1.2
==============

Author: @aitch_two  
URL: https://github.com/aitch-two/THETATools

フリーのペイント/レタッチソフトPaint.NET用の、
Ricoh THETAなどが出力する正距円筒図法（Equirectangular projection）の画像を
扱うプラグインです。

以下の4種類のプラグインを含みます。

- **水平調整（Level）**（EqRectLevel.dll）  
  傾いた正距円筒画像を調整して、水平な正距円筒画像を出力することができます。

- **透視図法変換（Perspective）**（PersProjection.dll）  
  通常の写真のような画像に変換します。

- **魚眼変換（FishEye）**（FishEyeProjection.dll）  
  3種類の投影法から選択して、魚眼レンズで撮影した写真のような画像に変換します。

- **メルカトル変換（Mercator）**（MercatorProjection.dll）  
  通常のパノラマ（正角円筒図法）のような画像に変換します。

インストール
------------

**Paint.NETをインストール**

1. Paint.NETをインストール  
  [http://www.getpaint.net/](http://www.getpaint.net/ "Paint.NET")

**THETAToolsプラグインをインストール**

1. THETATools1_1.zipをダウンロードして適当なフォルダに解凍  
[https://github.com/aitch-two/THETATools/releases](https://github.com/aitch-two/THETATools/releases "THETATools1_1.zip")  

2. 解凍したDLLファイルを、Paint.NETをインストールしたフォルダのEffectsフォルダにコピーする  
  通常は「C:\Program Files\Paint.NET\Effects」にコピー
3. Paint.NETを起動中なら、Paint.NETを再起動する

アンインストール
----------------

**THETAToolsプラグインをアンインストール**

1. Paint.NETを起動中なら、Paint.NETを終了する
2. Paint.NETをインストールしたフォルダのEffectsフォルダから、
  該当のDLLファイルを削除する  
  通常は「C:\Program Files\Paint.NET\Effects」から削除

**Paint.NETをアンインストール**

1. [コントロール パネル]-[プログラム]-[プログラムと機能]で、
  [Paint.NET]を選択して[アンインストール]

使い方
------

以下の項目は各プラグインに共通する項目です。

- **変換対象画像**  
  画像の縦横比にかかわらず、横-180°～+180°、縦-90°～+90°の
  正距円筒図法であるとみなします。

  Paint.NETプラグインは変換前と変換後で画像の大きさが同じになります。
  例えば変換後の画像を「3:2」にしたい場合は、
  変換前の正距円筒画像の縦を1.333倍に拡大して
  「3:2」にしておくといいでしょう。

- **逆変換（Invert convert）**  
  この項目がチェックされていると、逆変換を行います。

  次のような方法で、正距円筒画像の北極・南極を含む任意の位置に、
  文字や図形を書き込むことができます。

  1. 正距円筒画像を透視図法変換します  
   ※「Invert convert」のチェックは外します
  2. 透視図法画像に文字や図形を書き込みます
  3. ここで透視図法変換を呼び出すと、ダイアログボックスに1.で変換した時の
   パラメータが残っているので、二重変換になります
  4. ダイアログの「Invert convert」にチェックを入れると、
   1.の変換の逆変換になり、透視図法画像が正距円筒画像に戻ります  
   ※透視図法画像の画角の外側は透明になります
  5. 1.の変換前の正距円筒画像と重ねると、
   正距円筒図形に文字や図形を書き込んだことになります

  透視図法変換の代わりに魚眼変換やメルカトル変換を使うと、
  違った効果になります。

- **アンチエイリアス（MonteCarlo）**  
  この数値を増やすことによって、
  モンテカルロ法的な手法でアンチエイリアスを行います。

  ただし、数値に比例して処理量が増えるにもかかわらず、
  Ricoh THETAの画像の場合はほとんど効果を体感することができません。

  通常は「1」のままで問題ないでしょう。
  CGで作成したようなシャープな正距円筒画像を扱う場合は、
  効果を確認しながら数値を増やしてみてください。

###水平調整（[効果]-[THETA]-[Level...]）

傾いた正距円筒画像を調整して、水平な正距円筒画像を出力することができます。

Ricoh THETAが出力するJPEGファイルのEXIFデータには傾き情報が入っていますが、
Paint.NETのプラグインからEXIFデータにアクセスすることはできないようです。
Paint.NETで保存した画像にはEXIFデータが入っていませんが、
このプラグインで水平を調整しておけば、
Ricoh THETAに対応していない正距円筒画像表示ソフトでも正しく表示できます。

- **Zenith X**  
  Ricoh THETAが出力するJPEGのEXIFデータの「Zenith X」を入力します。  
  手動で調整する場合は、Directioinが0のとき画像中央が水平になるようにします。

- **Zenith Y**  
  Ricoh THETAが出力するJPEGのEXIFデータの「Zenith Y」を入力します。  
  手動で調整する場合は、Directioinを90にして、画像中央が水平になるようにします。

- **Direction**  
  ファーストビュー方向を指定します。  
  手動で調整する場合は「Zenith X」「Zenith Y」の調整が終わってから
  指定するようにします。

###透視図法変換（[効果]-[THETA]-[Perspective...]）

通常の写真のような画像に変換します。

- **X Direction**
- **Y Direction**
- **Z Direction**  
  変換後の画像の中央方向・画像の傾きを指定します

- **Focus Length**  
  画角を35mm換算の焦点距離のmm数で指定します。

###魚眼変換（[効果]-[THETA]-[FishEye...]）

魚眼レンズで撮影した写真のような画像に変換します。

- **X Direction**
- **Y Direction**
- **Z Direction**  
  変換後の画像の中央方向・画像の傾きを指定します

- **Projection**  
  投影方法を選択します。
  * Equidistance（等距離射影方式）  
   ほとんどの魚眼レンズで採用されている射影方式。  
   最大画角は360°で全天球を含むことができる。
  * Equisolid-angle（等立体角射影方式）  
   シグマ社の魚眼レンズで採用されている射影方式。  
   最大画角は360°で全天球を含むことができる。
   Equidistance（等距離射影方式）と比較すると、周辺部分が圧縮されている。
  * Orthographic（正射影方式）  
   特殊な魚眼レンズで採用されている射影方式。
   最大画角は180°。

- **Magnification**  
  拡大率。変換結果の大きさを調整します。

###メルカトル変換（[効果]-[THETA]-[Mercator...]）

通常のパノラマ（正角円筒図法）のような画像に変換します。

変換後の画像の横方向の画角は360°固定です。
縦方向の画角は画像の縦サイズで決まります。
2:1の正距円筒画像から変換する場合、縦の画角は35mm換算で7.6mm程度となります。

- **X Direction**
- **Y Direction**
- **Z Direction**  
   変換後の画像の中央方向・画像の傾きを指定します


ビルド方法
---------

これらのプラグインはPaint.NETの「プラグインを作成するプラグイン」
CodeLab for Paint.NETで作成しました。   
[http://www.boltbait.com/pdn/codelab/](http://www.boltbait.com/pdn/codelab/ "CodeLab for Paint.NET")

**CodeLab for Paint.NETをインストール**

1. CodeLab for Paint.NETをインストール  
  [http://www.boltbait.com/pdn/codelab/](http://www.boltbait.com/pdn/codelab/ "CodeLab for Paint.NET")

**プラグインをビルド**

1. 起動しているPaint.NETをすべて終了する
2. ビルドしようとするプラグインと同じ名前のDLLを、
  Paint.NETをインストールしたフォルダのEffectsフォルダから削除する  
  通常は「C:\Program Files\Paint.NET\Effects」から削除
3. Paint.NETを**管理者権限**で起動する
4. [効果]-[Advanced]-[CodeLab]を選択する
5. [CodeLab]ウィンドウで[File]-[Open...]を選択する
6. ソースファイルを選択して開く
7. [File]-[Save As DLL...]を選択する
8. [Building xxx.dll]-[Select Icon]をクリックしてPNGファイルを指定する
9. [Build]ボタンをクリック
10. 「Build succeeded!」が表示される
11. [OK]ボタンをクリックして[Build Finished]ダイアログを閉じる
12. [Cancel]ボタンをクリックして[CodeLab]ウィンドウを閉じる
13. Paint.NETを再起動する（管理者権限でなくても大丈夫）
14. プラグインのDLLが、
  Paint.NETをインストールしたフォルダのEffectsフォルダに作成されている  
  通常は「C:\Program Files\Paint.NET\Effects」


履歴
----

2014-01-12 Version 1.2 処理の効率化・魚眼変換のクラス構成の整理

2013-12-28 Version 1.1 メルカトル変換（MercatorProjection.dll）の逆変換のバグの修正

2013-12-21 Version 1.0 初版公開

以上