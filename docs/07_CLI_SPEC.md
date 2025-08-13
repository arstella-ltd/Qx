# Qx CLI仕様書

---
version: 2.0.0
last_updated: 2025-08-13
author: Development Team
status: approved
---

## 📌 概要

Qx (Query eXpress) は、OpenAI APIを活用したインテリジェントCLIツールです。
自然言語での質問に対して、Web検索や関数呼び出しを自動実行し、構造化された回答を提供します。

### 対象読者
- CLIツールのエンドユーザー
- Qxを統合する開発者
- AIエージェント開発者

### 前提条件
- 環境変数 `OPENAI_API_KEY` が設定されていること
- .NET 9.0ランタイムまたはネイティブバイナリ

## 🎯 コマンド構造

### 基本構文

```bash
qx <prompt> [options]
```

### 標準入力対応

```bash
# パイプ経由
echo "prompt" | qx [options]
cat file.txt | qx [options]

# リダイレクト経由
qx [options] < prompt.txt

# 組み合わせ（標準入力 + コマンドライン引数）
echo "context" | qx "additional prompt" [options]
```

## 📊 引数とオプション仕様

### 位置引数

| 引数 | 必須 | 型 | 説明 | 制約 |
|------|------|-----|------|------|
| `prompt` | ❌ | string | 自然言語プロンプト | 標準入力がある場合は省略可能 |

### グローバルオプション

| オプション | 短縮形 | 型 | デフォルト | 説明 | 環境変数 |
|-----------|--------|-----|------------|------|----------|
| `--model` | `-m` | string | gpt-5-nano | 使用するAIモデル | QX_MODEL |
| `--output` | `-o` | string | なし | 出力ファイルパス | - |
| `--temperature` | `-t` | double | 1.0 | 応答生成の温度 (0.0-2.0) | QX_TEMPERATURE |
| `--max-tokens` | - | int? | なし | 最大トークン数 | QX_MAX_TOKENS |
| `--web-search` | `-w` | bool | true | Web検索を有効化 | - |
| `--no-web-search` | - | bool | false | Web検索を無効化 | - |
| `--functions` | `-f` | bool | true | 関数呼び出しを有効化 | - |
| `--no-functions` | - | bool | false | 関数呼び出しを無効化 | - |
| `--verbose` | `-v` | bool | false | 詳細出力モード | QX_VERBOSE |
| `--version` | - | bool | false | バージョン情報表示 | - |
| `--help` | `-h` | bool | false | ヘルプ表示 | - |

### 利用可能なモデル

| モデル名 | 説明 | 推奨用途 |
|---------|------|----------|
| gpt-5-nano | 高速・軽量モデル（デフォルト） | 簡単な質問、計算 |
| gpt-5-mini | バランス型モデル | 一般的な質問 |
| gpt-5 | 高性能モデル | 複雑な推論 |
| gpt-4o | GPT-4 Omniモデル | 高度な分析 |
| gpt-4o-mini | GPT-4 Omni軽量版 | 効率的な処理 |
| o3 | 最新実験モデル | 最先端機能 |
| gpt-4.1 | GPT-4.1モデル | 改良版推論 |
| gpt-4.1-mini | GPT-4.1軽量版 | 効率重視 |
| gpt-4.1-nano | GPT-4.1超軽量版 | 高速処理 |

## 🔧 機能仕様

### Web検索機能

有効時（デフォルト）、LLMが必要に応じて自動的にWeb検索を実行します。

```bash
# Web検索有効（デフォルト）
qx "最新のReact 19の新機能"

# 明示的に有効化
qx "最新情報" --web-search

# 無効化（モデルの知識のみ使用）
qx "Pythonの基本構文" --no-web-search
```

### 関数呼び出し機能

利用可能な関数：

| 関数名 | 説明 | パラメータ | 戻り値例 |
|--------|------|------------|----------|
| GetCurrentTime | 指定タイムゾーンの現在時刻 | timezone (string) | "Current time in JST: 2025-08-13 15:30:00" |
| GetWeather | 指定地域の天気情報（モック） | location (string), unit (string) | "Weather in Tokyo: Clear, 20°C" |
| CalculateExpression | 数式の計算 | expression (string) | "Result: 42" |

```bash
# 関数呼び出し有効（デフォルト）
qx "今の東京の時間は？"
# 出力: Current time in Asia/Tokyo: 2025-08-13 15:30:00

# 無効化
qx "今の時間は？" --no-functions
# 出力: 現在の時刻を取得することはできません...
```

## 📤 入出力仕様

### 入力形式

1. **コマンドライン引数**: UTF-8文字列
2. **標準入力**: UTF-8テキストストリーム
3. **ファイルリダイレクト**: UTF-8エンコードファイル

優先順位：
- 標準入力とコマンドライン引数が両方ある場合、標準入力が先、引数が後に結合

### 出力形式

#### 通常モード（デフォルト）

```
<応答テキスト>
```

#### Verboseモード（--verbose）

```
Processing query with model: <model>
Temperature: <temp>, Max tokens: <tokens>
Web search: <status>
Function calling: <status>

=== Response Details (JSON) ===
{
  "Model": "...",
  "RequestOptions": {...},
  "ResponseMetadata": {...}
}

=== Response Text ===
Response:
[Function Call: <name>]  # 関数呼び出しがある場合のみ
<応答テキスト>
```

#### ファイル出力（--output）

```
Response saved to: <filepath>
```

## 🔄 終了コードと例

| コード | 意味 | 発生条件 | 対処法 |
|--------|------|----------|--------|
| 0 | 成功 | 正常終了 | - |
| 1 | 一般エラー | プロンプト未指定等 | 引数を確認 |
| 2 | APIエラー | OpenAI API呼び出し失敗 | APIキー、ネットワークを確認 |
| 3 | 環境エラー | APIキー未設定 | OPENAI_API_KEY を設定 |
| 4 | ファイルアクセスエラー | 出力ファイル書き込み失敗 | 権限、パスを確認 |
| 5 | I/Oエラー | 入出力エラー | ディスク容量等を確認 |
| 124 | タイムアウト | リクエストタイムアウト | ネットワーク、--max-tokensを確認 |

## 💡 使用例

### 基本的な使用

```bash
# シンプルな質問
qx "What is 2+2?"
# 出力: 4

# 日本語での質問
qx "フィボナッチ数列の最初の10個"
# 出力: 0, 1, 1, 2, 3, 5, 8, 13, 21, 34
```

### パイプとの連携

```bash
# ディレクトリ内容の分析
ls -la | qx "このディレクトリの構造を説明"

# Git差分のレビュー
git diff | qx "変更内容をレビュー"

# ログ分析
tail -100 error.log | qx "エラーパターンを分析"
```

### オプションの組み合わせ

```bash
# 高性能モデルで詳細出力
qx "複雑な数学問題" --model gpt-4o --verbose

# 結果をファイルに保存
qx "APIドキュメント生成" --output api-docs.md

# 低温度で確定的な回答
qx "法的文書の要約" --temperature 0.2
```

### AIエージェントからの利用

```bash
# 構造化出力を期待するエージェント
response=$(qx "データを分析" --no-web-search)

# パイプラインでの利用
curl -s https://api.example.com/data | \
  qx "JSONをYAMLに変換" | \
  kubectl apply -f -
```

## ⚙️ 設定と環境

### 環境変数

| 変数名 | 必須 | 説明 | デフォルト | 例 |
|--------|------|------|------------|-----|
| OPENAI_API_KEY | ✅ | OpenAI APIキー | なし | sk-... |
| QX_MODEL | ❌ | デフォルトモデル | gpt-5-nano | gpt-4o |
| QX_TEMPERATURE | ❌ | デフォルト温度 | 1.0 | 0.7 |
| QX_MAX_TOKENS | ❌ | デフォルト最大トークン | なし（無制限） | 500 |
| QX_VERBOSE | ❌ | 詳細出力モード | false | true |
| QX_TIMEOUT | ❌ | タイムアウト（秒） | 60 | 120 |

### 設定の優先順位

```
1. コマンドライン引数（最優先）
2. 環境変数
3. デフォルト値
```

## 🚀 パフォーマンス仕様

### リソース要件

| 項目 | 最小要件 | 推奨 |
|------|----------|------|
| メモリ | 50MB | 100MB |
| CPU | 1コア | 2コア |
| ネットワーク | 1Mbps | 10Mbps |
| 起動時間 | < 100ms | < 50ms |

### タイムアウト

- デフォルト: 60秒
- 最大: 600秒（環境変数で設定可能）
- Web検索時は自動延長される場合あり

## 🔐 セキュリティ

### APIキー管理

- 環境変数経由でのみ設定（コマンドライン引数では受け付けない）
- キーはメモリ内でのみ保持
- ログ出力時は自動マスキング

### データ処理

- 入力データはOpenAI APIにのみ送信
- ローカルには保存されない（--outputオプション使用時を除く）
- SSL/TLS暗号化通信

## 📝 非推奨と移行

### 将来の変更予定

現在、非推奨の機能はありません。

### バージョニング方針

- メジャー: 破壊的変更
- マイナー: 機能追加（後方互換）
- パッチ: バグ修正

## 🧪 テストと検証

### 動作確認コマンド

```bash
# バージョン確認
qx --version

# 基本動作テスト
echo "test" | qx

# 関数呼び出しテスト
qx "What time is it?" --verbose

# Web検索テスト
qx "Latest news" --verbose
```

### 統合テスト

プロジェクトの `Qx.Tests` で自動テストを実行：

```bash
dotnet test
```

## 📚 関連ドキュメント

- [EXAMPLE.md](./EXAMPLE.md) - 詳細な使用例集
- [TECH.md](./TECH.md) - 技術仕様
- [README.md](../README.md) - クイックスタート

## 🔄 変更履歴

| 日付 | バージョン | 変更内容 | 変更者 |
|------|------------|----------|--------|
| 2025-08-13 | 2.0.0 | 標準入力対応、Function Call表示制御追加 | Development Team |
| 2025-08-12 | 1.0.0 | 初版作成 | Development Team |

---

*この仕様書は実装と同期して更新してください。疑問点があれば Issue を作成してください。*