# Qx 使用例集

---
version: 1.1.0
last_updated: 2025-08-13
author: Development Team  
status: approved
---

## 基本的な使い方

### シンプルな質問
```bash
qx "What is 2+2?"
# 出力: 4

qx "今の時間は？"
# 出力: Current time in UTC: 2025-08-13 12:00:00
```

### 計算機能の使用
```bash
qx "Calculate 123 * 456"
# 出力: Result: 56088

qx "10の階乗を計算して"
# 出力: 3628800
```

## 標準入力からの入力

### パイプ経由での入力
```bash
# シンプルな例
echo "What is the current time in Tokyo?" | qx
# 出力: Current time in JST: 2025-08-13 15:19:36

# ファイルの内容を解析
cat README.md | qx "このドキュメントを要約して"

# コマンド出力を解析
ls -la | qx "このディレクトリの内容を説明して"

# gitの差分を解析
git diff | qx "この変更内容を要約して"
git log --oneline -10 | qx "最近のコミット傾向を分析して"
```

### ファイルリダイレクト経由での入力
```bash
# プロンプトファイルから読み込み
qx < prompt.txt

# 複数行のプロンプト
cat > long_prompt.txt << EOF
これは複数行のプロンプトです。
以下の要件を満たすPythonスクリプトを作成してください：
1. CSVファイルを読み込む
2. データを集計する
3. グラフを生成する
EOF
qx < long_prompt.txt
```

### 標準入力とコマンドライン引数の組み合わせ
```bash
# 標準入力の内容に対して、コマンドライン引数で指示を与える
ls -la | qx "ファイルサイズの大きい順に並べ替えて説明"

# プロセスリストを解析
ps aux | qx "メモリ使用量の多いプロセストップ5を教えて"

# ログファイルの解析
tail -100 /var/log/syslog | qx "エラーや警告を抽出して要約"

# コードレビュー
git diff HEAD~1 | qx "このコードの改善点を指摘して"
```

## オプションの使用

### モデルの指定
```bash
qx "複雑な数学の問題" --model gpt-4o
qx "簡単な質問" -m gpt-5-nano
```

### 温度パラメータの調整
```bash
# より創造的な回答（高温度）
qx "物語を書いて" --temperature 1.5

# より確定的な回答（低温度）
qx "2+2の答えは？" -t 0.1
```

### 詳細出力（verbose）モード
```bash
qx "What is 2+2?" --verbose
# 出力: Processing query with model: gpt-5-nano
#       Temperature: 1, Max tokens: unlimited
#       Web search: Enabled (Note: Not all models support web search)
#       Function calling: Enabled (GetCurrentTime, GetWeather, CalculateExpression)
#       
#       === Response Details (JSON) ===
#       {...}
#       === Response Text ===
#       Response:
#       4

# 関数呼び出しを含む場合
qx "What time is it?" --verbose
# 出力: ...
#       === Response Text ===
#       Response:
#       [Function Call: GetCurrentTime]
#       Current time in UTC: 2025-08-13 12:00:00
```

### Web検索の有効/無効
```bash
# Web検索を無効にする（モデルの知識のみ使用）
qx "最新のニュースを教えて" --no-web-search

# Web検索を明示的に有効にする
qx "2024年のオリンピックについて" --web-search
```

### 関数呼び出しの有効/無効
```bash
# 関数呼び出しを無効にする
qx "What time is it?" --no-functions

# 関数呼び出しを明示的に有効にする
qx "Calculate 100 * 200" --functions
```

### 出力をファイルに保存
```bash
qx "長い説明を生成して" --output result.txt
qx "APIドキュメントを生成" -o api-docs.md
```

### 最大トークン数の制限
```bash
qx "詳細な説明" --max-tokens 100
qx "簡潔な要約" --max-tokens 50
```

## 実用的な使用例

### コードレビュー
```bash
# 特定のファイルの変更をレビュー
git diff main feature-branch -- src/main.py | qx "このコードの潜在的な問題を指摘して"

# 最新のコミットをレビュー
git show HEAD | qx "このコミットの変更内容をレビューして"
```

### ドキュメント生成
```bash
# 関数のドキュメント生成
cat myfunction.py | qx "この関数のdocstringを生成して"

# READMEの生成
ls -la src/ | qx "このプロジェクト構造に基づいてREADMEの概要セクションを作成"
```

### データ分析
```bash
# CSVデータの分析
head -20 data.csv | qx "このデータの傾向を分析して"

# ログ分析
grep ERROR app.log | tail -50 | qx "エラーパターンを分析して原因を推測"
```

### システム管理
```bash
# ディスク使用状況の分析
df -h | qx "ディスク使用状況を分析して、問題があれば指摘して"

# プロセス監視
top -b -n 1 | qx "システムリソースの使用状況を要約"

# ネットワーク診断
netstat -an | qx "異常な接続がないか確認"
```

### 翻訳とフォーマット変換
```bash
# 翻訳
echo "This is a test message" | qx "日本語に翻訳して"

# JSONからYAMLへの変換
cat config.json | qx "このJSONをYAML形式に変換"

# Markdownからプレーンテキストへ
cat document.md | qx "プレーンテキストに変換して要点をまとめて"
```

## 複数のオプションを組み合わせた使用例

```bash
# 詳細モードで、モデルを指定し、出力をファイルに保存
git diff | qx "コードレビュー" --model gpt-4o --verbose --output review.md

# Web検索を無効にし、低温度で確定的な回答を得る
echo "Pythonでフィボナッチ数列" | qx --no-web-search --temperature 0.2

# 関数呼び出しを有効にし、結果を簡潔に
qx "東京の天気は？" --functions --max-tokens 50
# 出力: Weather in Tokyo, Japan: Clear, 20°C
```

## パイプラインでの使用

```bash
# 複数のコマンドを連鎖
curl -s https://api.example.com/data | jq '.items[]' | qx "このデータを分析"

# 処理結果を次のコマンドへ
qx "Generate 10 random numbers" | sort -n | qx "統計情報を計算"

# ファイル処理パイプライン
find . -name "*.py" -exec cat {} \; | qx "Pythonコードの品質を評価"
```

## エラー処理とデバッグ

```bash
# verboseモードでデバッグ情報を確認
echo "複雑なクエリ" | qx --verbose 2>&1 | tee debug.log

# エラー出力の確認
qx "無効なリクエスト" 2> error.log

# タイムアウトの設定（システムレベル）
timeout 30 qx "長時間かかる処理"
```

## バッチ処理

```bash
# 複数のプロンプトを順次処理
for prompt in "質問1" "質問2" "質問3"; do
    echo "$prompt" | qx >> results.txt
    echo "---" >> results.txt
done

# ファイルリストの処理
find . -name "*.md" | while read file; do
    cat "$file" | qx "要約して" > "${file%.md}_summary.txt"
done
```

## 環境変数の活用

```bash
# OpenAI APIキーの設定（必須）
export OPENAI_API_KEY="your-api-key-here"

# デフォルトモデルの設定（オプション）
export QX_DEFAULT_MODEL="gpt-4o"
export QX_DEFAULT_TEMPERATURE="0.7"
```

## Tips & Tricks

1. **簡潔な出力を得る**: verboseフラグを使わない場合、結果のみが表示されます
2. **パイプとリダイレクトの使い分け**: 動的な出力にはパイプ、静的なファイルにはリダイレクト
3. **関数呼び出しの活用**: 時刻、天気、計算などは自動的に関数が呼ばれます
4. **モデル選択**: 複雑なタスクには`gpt-4o`、簡単なタスクには`gpt-5-nano`
5. **出力の再利用**: `-o`オプションで結果を保存し、後で参照可能