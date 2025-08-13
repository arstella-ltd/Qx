# Qx 技術仕様書

---
version: 1.0.0
last_updated: 2025-08-12
author: Development Team
status: draft
---

## 🛠️ 技術スタック

### コア技術

| 技術 | バージョン | 選定理由 | ライセンス |
|------|------------|----------|------------|
| .NET | 9.0 | AoTコンパイル対応、高性能なネイティブバイナリ生成 | MIT |
| C# | 13 | 最新言語機能によるモダンな開発体験 | MIT |
| OpenAI SDK | 2.3.0 | 公式SDKによる安定したAPI連携 | MIT |
| System.CommandLine | 2.0.0-beta6 | .NET標準のCLIフレームワーク | MIT |

### 開発・テスト

| 技術 | バージョン | 用途 | ライセンス |
|------|------------|------|------------|
| xUnit | 2.9.3 | 単体テストフレームワーク | Apache 2.0 |
| FluentAssertions | 7.0.0 | テストアサーション | Apache 2.0 |
| Moq | 4.20.72 | モックフレームワーク | BSD |

## 💻 開発環境

### 必要なツール

```bash
# 必須
- .NET SDK 9.0以降
- Git 2.25以降
- Visual Studio Code

# 推奨拡張機能（VS Code）
- C# Dev Kit
- .NET Install Tool
- GitLens
```

### セットアップ手順

```bash
# 1. リポジトリのクローン
git clone https://github.com/[org]/qx.git
cd qx

# 2. 依存関係の復元
dotnet restore

# 3. ビルド確認
dotnet build

# 4. テスト実行
dotnet test

# 5. ローカル実行
dotnet run -- "your query here"
```

### 環境変数

| 変数名 | 必須 | 説明 | デフォルト |
|--------|------|------|------------|
| OPENAI_API_KEY | ✅ | OpenAI APIキー | なし |
| QX_TIMEOUT | ❌ | APIタイムアウト（秒） | 60 |
| QX_DEBUG | ❌ | デバッグモード | false |

## ⚠️ System.CommandLine 注意事項

### API設計の重要ポイント

System.CommandLine は現在ベータ版（v2.0.0-beta6）のため、以下の点に注意が必要：

#### 1. Option コンストラクタのオーバーロード

```csharp
// ❌ 誤り：名前付きパラメータは使用できない
var option = new Option<string>(
    name: "--model",
    getDefaultValue: () => "gpt-3.5-turbo",
    description: "Model to use");

// ✅ 正しい：順序を守った引数
var option = new Option<string>(
    new[] { "--model", "-m" },
    () => "gpt-3.5-turbo",
    "Model to use");

// ✅ またはプロパティ初期化子を使用
var option = new Option<string>("--model")
{
    Description = "Model to use"
};
```

#### 2. SetAction vs SetHandler

```csharp
// SetAction: 同期処理、ParseResultを受け取る
command.SetAction((parseResult) => 
{
    // 同期処理のみ
    return 0;
});

// SetHandler: 型安全なパラメータバインディング（推奨）
command.SetHandler(
    async (string arg1, int arg2) => 
    {
        // 非同期処理可能
    },
    argument1,
    option2);
```

#### 3. 非同期処理の扱い

```csharp
// ❌ 誤り：SetActionは非同期ラムダを直接サポートしない
command.SetAction(async (parseResult) => 
{
    await SomeAsyncMethod();
    return 0; // コンパイルエラー
});

// ✅ 正しい：Task.Runで包む
command.SetAction((parseResult) => 
{
    Task.Run(async () => await handler.HandleAsync())
        .GetAwaiter().GetResult();
    return 0;
});
```

#### 4. Arguments と Options の追加

```csharp
// Command クラスのプロパティを使用
command.Arguments.Add(argument);  // Arguments コレクション
command.Options.Add(option);      // Options コレクション

// 注意：AddArgument/AddOption メソッドは存在しない
```

#### 5. デフォルト値の設定

```csharp
// Option<T> のデフォルト値は型によって異なる動作
// string: null, int: 0, bool: false

// 明示的にデフォルト値を確認
if (temperature == 0) // double型のデフォルト
{
    temperature = 0.7;
}
```

#### 6. RootCommand の実行

```csharp
// ❌ 誤り：Invoke メソッドは直接存在しない
rootCommand.Invoke(args);

// ✅ 正しい：Parse してから Invoke
rootCommand.Parse(args).Invoke();

// または CommandLineBuilder を使用（より柔軟）
new CommandLineBuilder(rootCommand)
    .UseDefaults()
    .Build()
    .Invoke(args);
```

### 移行時の注意事項

System.CommandLine は正式版リリース時に API が変更される可能性があります：

1. **破壊的変更の可能性**: ベータ版のため、メジャーアップデート時は要注意
2. **ドキュメント不足**: 公式ドキュメントが不完全な箇所がある
3. **パフォーマンス**: AoT コンパイル時のリフレクション使用に注意
4. **依存性注入**: 標準的なDIコンテナとの統合は手動実装が必要

### 推奨プラクティス

1. **コマンドの分離**: 各コマンドを独立したクラスに実装
2. **ハンドラーパターン**: ビジネスロジックをハンドラーに分離
3. **DIコンテナ統合**: ServiceProviderを使用した依存性注入
4. **エラーハンドリング**: 終了コードを適切に設定

## 🔨 ビルド・デプロイ

### ローカルビルド

```bash
# デバッグビルド
dotnet build

# リリースビルド（AoT）
dotnet publish -c Release -r <RID> --self-contained /p:PublishAot=true

# 各プラットフォーム向けRID
# macOS Universal: osx-arm64 と osx-x64 を個別ビルド後、lipo で結合
# Windows: win-x64
# Linux: linux-x64
```

### macOS Universal Binary作成

```bash
# ARM64ビルド
dotnet publish -c Release -r osx-arm64 --self-contained /p:PublishAot=true -o ./publish/osx-arm64

# x64ビルド  
dotnet publish -c Release -r osx-x64 --self-contained /p:PublishAot=true -o ./publish/osx-x64

# Universal Binary作成
lipo -create ./publish/osx-arm64/qx ./publish/osx-x64/qx -output ./publish/qx-universal
```

### プロジェクト設定

- **ターゲットフレームワーク**: net9.0
- **AoT設定**: PublishAot有効
- **トリミング**: フルトリミングモード
- **シンボル**: ストリップ有効
- **グローバリゼーション**: 有効（多言語対応）
- **アナライザー**: AoT/Trim互換性チェック有効

### CI/CD（GitHub Actions）

**ワークフロー構成:**
- トリガー: タグプッシュ（v*）
- ビルドマトリックス: Linux x64、Windows x64、macOS Universal
- .NET 9.0環境でAoTビルド実行
- アーティファクトをGitHub Releasesへアップロード

### パッケージマネージャー配布

#### Homebrew（macOS/Linux）

- **Formula名**: qx
- **配布形式**: tar.gz圧縮
- **macOS**: Universal Binary配布
- **Linux**: x64バイナリ配布
- **インストール先**: /usr/local/bin/qx

#### asdf（クロスプラットフォーム）

- **プラグイン名**: asdf-qx
- **バージョン管理**: .tool-versionsファイル対応
- **インストール方式**: GitHub Releasesからダウンロード
- **対応OS**: Linux、macOS、Windows（WSL）

#### Scoop（Windows）

- **バケット**: main または extras
- **配布形式**: zip圧縮
- **アーキテクチャ**: x64のみ
- **実行ファイル**: qx.exe
- **自動更新**: checkverとautoupdate対応

## 📊 運用要件

### 動作環境

| OS | バージョン | アーキテクチャ | 備考 |
|----|------------|---------------|------|
| Linux | Ubuntu 20.04+, RHEL 8+ | x64 | glibc 2.31+ |
| macOS | 12.0+ | Universal (ARM64/x64) | Rosetta 2対応 |
| Windows | 10 (1809+), 11 | x64 | .NET不要 |

### 必要リソース

| リソース | 最小要件 | 推奨 | 備考 |
|----------|----------|------|------|
| CPU | 1コア | 2コア | ARM64/x64対応 |
| メモリ | 128MB | 256MB | AoTにより軽量 |
| ディスク | 50MB | 100MB | 単一バイナリ |
| ネットワーク | 1Mbps | 10Mbps | API通信用 |

### パフォーマンス特性

| 項目 | 目標値 | 実測値（参考） |
|------|--------|---------------|
| 起動時間 | < 100ms | 50-80ms |
| メモリ使用量 | < 200MB | 100-150MB |
| バイナリサイズ | < 30MB | 20-25MB |
| APIレスポンス | < 30s | 5-25s |

### 監視項目

本ツールはCLIアプリケーションのため、常時監視は不要。以下の項目を出力で確認：

```bash
# 標準出力
- API応答結果
- 処理完了メッセージ

# 標準エラー出力
- エラーメッセージ
- API接続失敗
- タイムアウト

# 終了コード
- 0: 成功
- 1: 一般的なエラー
- 2: API接続エラー
- 3: 認証エラー
- 124: タイムアウト
```

## 🧪 テスト戦略

### TDD実践

**t-wada流TDDサイクル:**
1. **Red**: 失敗するテストを最初に書く
2. **Green**: テストを通す最小限の実装
3. **Refactor**: コードの品質向上

**テスト原則:**
- テストファースト開発
- 小さなステップで進める
- テストの可読性を重視
- モックは最小限に

### テストカバレッジ

| 種別 | 目標 | 備考 |
|------|------|------|
| 単体テスト | 60%+ | コアロジック重視 |
| 統合テスト | 主要パス | E2Eシナリオ |
| 手動テスト | リリース前 | 各プラットフォーム |

## 🔐 セキュリティ考慮事項

### APIキー管理

**取得方法:**
- 環境変数 OPENAI_API_KEY からのみ読み込み

**セキュリティポリシー:**
- メモリ内でのみ保持
- ログ出力禁止
- エラーメッセージに含めない
- 設定ファイルへの保存禁止

### ビルド時セキュリティ

**リリースビルド設定:**
- デバッグ情報: 削除
- シンボル: ストリップ
- 最適化: 最大レベル
- 難読化: AoTによる自然な難読化

## 📦 依存関係管理

### NuGetパッケージ

**本番依存:**
- OpenAI: v2.3.0（公式SDK、2025年8月最新）
- System.CommandLine: v2.0.0-beta6.25358.103（CLIフレームワーク、最新ベータ）

**開発・テスト依存:**
- xUnit: v2.9.3（テストフレームワーク、最新安定版）
- xUnit.runner.visualstudio: v2.8.2（テストランナー）
- FluentAssertions: v7.0.0（アサーション、商用利用でも無料のバージョン）
- Moq: v4.20.72（モック、2024年9月更新）

**注記:**
- FluentAssertions v8以降は商用利用に有料ライセンスが必要なため、v7.0.0を使用
- xUnit v3も利用可能だが、安定性を重視してv2系を採用

### 更新ポリシー

- **メジャー更新**: 慎重に評価、破壊的変更の確認
- **マイナー更新**: 月次で確認、テスト後適用
- **パッチ更新**: セキュリティ修正は即座に適用

## 🚀 リリース手順

### バージョニング

セマンティックバージョニング（SemVer）を採用：
- **MAJOR.MINOR.PATCH** (例: 1.2.3)
- MAJOR: 破壊的変更
- MINOR: 機能追加
- PATCH: バグ修正

### リリースフロー

```bash
# 1. バージョンタグ作成
git tag -a v1.0.0 -m "Release version 1.0.0"
git push origin v1.0.0

# 2. GitHub Actions が自動実行
# - 各プラットフォーム向けビルド
# - GitHub Releases作成
# - パッケージマネージャー更新PR

# 3. リリースノート作成
# - 新機能
# - バグ修正
# - 破壊的変更
```

## 📝 メンテナンス

### 定期作業

| 頻度 | 作業内容 |
|------|----------|
| 週次 | 依存関係の脆弱性チェック |
| 月次 | パッケージ更新確認 |
| 四半期 | .NET SDKバージョン確認 |
| 年次 | メジャーバージョン計画 |

## 🔄 変更履歴

| 日付 | バージョン | 変更内容 | 変更者 |
|------|------------|----------|--------|
| 2025-08-12 | 1.0.0 | 初版作成 | Development Team |

---

*このドキュメントは技術仕様に焦点を当てています。機能要求は [REQUIREMENTS.md](./REQUIREMENTS.md)、製品ビジョンは [PRODUCT.md](./PRODUCT.md) を参照してください。*