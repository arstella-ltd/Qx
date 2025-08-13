# Qx (Query eXpress)

OpenAI APIを活用した高度なWeb検索機能を持つインテリジェントCLIツール。

## 特徴

- 🚀 **シンプルなコマンド**: `qx "質問"` で即座に実行
- 🔍 **Web検索統合**: 最新情報を自動的に検索して回答
- ⚡ **高速起動**: AoTコンパイルによるネイティブバイナリ
- 🎯 **柔軟な設定**: モデル、温度、トークン数などをカスタマイズ可能

## インストール

### 必要要件

- OpenAI APIキー
- 環境変数 `OPENAI_API_KEY` の設定

### ビルド方法

```bash
git clone https://github.com/[org]/qx.git
cd qx
dotnet build
```

## 使い方

### 基本的な使用方法

```bash
# シンプルな質問
qx "What is 2+2?"

# Web検索を有効にして最新情報を取得（デフォルトで有効）
qx "What's happening in technology today?"

# Web検索を無効化
qx "Explain quantum computing" --no-web-search

# モデルを指定
qx "Write a haiku" --model gpt-4o

# 詳細な設定
qx "Generate code" --temperature 0.5 --max-tokens 2000
```

### オプション

| オプション | 短縮形 | 説明 | デフォルト |
|----------|--------|------|----------|
| --model | -m | 使用するAIモデル | gpt-5 |
| --output | -o | 出力ファイルパス | - |
| --temperature | -t | 応答生成の温度 (0.0-2.0) | 1.0 |
| --max-tokens | - | 最大トークン数 | 1000 |
| --web-search | -w | Web検索を有効化 | 有効 |
| --no-web-search | - | Web検索を無効化 | - |
| --version | - | バージョン情報を表示 | - |
| --help | -h | ヘルプを表示 | - |

## Web検索機能

Qxは、OpenAI Response APIのWeb検索ツールを使用して、最新の情報を取得できます。
この機能により、モデルの知識カットオフ以降の出来事や、リアルタイムの情報について質問できます。

**注意**: Web検索機能は特定のモデル（gpt-4o-mini等）でのみ利用可能です。

## ライセンス

MIT License

## 貢献

プルリクエストを歓迎します。大きな変更の場合は、まずissueを開いて変更内容について議論してください。