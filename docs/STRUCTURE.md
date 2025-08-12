# Qx プロジェクト構造

---
version: 1.0.0
last_updated: 2025-08-12
author: Development Team
status: draft
---

## 📁 ディレクトリ構造

```
Qx/
├── src/
│   └── Qx/
│       ├── Command/                    # CLIコマンド実装
│       │   ├── QueryCommand.cs
│       │   ├── HelpCommand.cs
│       │   └── VersionCommand.cs
│       ├── Service/                    # ビジネスロジック
│       │   ├── IOpenAIService.cs
│       │   ├── OpenAIService.cs
│       │   ├── IRetryService.cs
│       │   └── RetryService.cs
│       ├── Model/                      # データモデル
│       │   ├── Query.cs
│       │   ├── Response.cs
│       │   ├── Configuration.cs
│       │   └── ApiOption.cs
│       ├── Handler/                    # 処理ハンドラー
│       │   ├── ICommandHandler.cs
│       │   ├── CommandHandler.cs
│       │   └── StreamHandler.cs
│       ├── Formatter/                  # 出力フォーマッター
│       │   ├── IOutputFormatter.cs
│       │   ├── ConsoleFormatter.cs
│       │   └── ErrorFormatter.cs
│       ├── Validator/                  # 入力検証
│       │   ├── IInputValidator.cs
│       │   └── InputValidator.cs
│       ├── Extension/                  # 拡張メソッド
│       │   ├── StringExtensions.cs
│       │   └── TaskExtensions.cs
│       ├── Exception/                  # カスタム例外
│       │   ├── QxException.cs
│       │   └── ApiException.cs
│       ├── Program.cs                  # エントリーポイント
│       └── Qx.csproj                   # プロジェクトファイル
│
├── test/
│   ├── Qx.Tests/
│   │   ├── Command/                    # コマンドテスト
│   │   │   └── QueryCommandTests.cs
│   │   ├── Service/                    # サービステスト
│   │   │   └── OpenAIServiceTests.cs
│   │   ├── Model/                      # モデルテスト
│   │   │   └── QueryTests.cs
│   │   ├── Handler/                    # ハンドラーテスト
│   │   │   └── CommandHandlerTests.cs
│   │   ├── Validator/                  # バリデーターテスト
│   │   │   └── InputValidatorTests.cs
│   │   ├── Helper/                     # テストヘルパー
│   │   │   ├── TestBase.cs
│   │   │   └── MockFactory.cs
│   │   └── Qx.Tests.csproj
│   │
│   └── Qx.IntegrationTests/
│       ├── ApiIntegrationTests.cs      # API統合テスト
│       ├── EndToEndTests.cs            # E2Eテスト
│       ├── PerformanceTests.cs         # パフォーマンステスト
│       └── Qx.IntegrationTests.csproj
│
├── docs/                                # ドキュメント
│   ├── PRODUCT.md
│   ├── REQUIREMENTS.md
│   ├── TECH.md
│   ├── DESIGN.md
│   └── STRUCTURE.md
│
├── .github/
│   └── workflows/                      # GitHub Actions
│       ├── ci.yml                      # CI パイプライン
│       ├── release.yml                 # リリースワークフロー
│       └── codeql.yml                  # セキュリティ分析
│
├── script/                              # ビルド・デプロイスクリプト
│   ├── build.sh                        # ビルドスクリプト
│   ├── test.sh                         # テスト実行スクリプト
│   └── publish.sh                      # パブリッシュスクリプト
│
├── .editorconfig                        # エディター設定
├── .gitignore                          # Git除外設定
├── Directory.Build.props               # 共通ビルド設定
├── Directory.Packages.props           # 集中パッケージ管理
├── global.json                         # .NET SDK バージョン
├── nuget.config                        # NuGet設定
├── LICENSE                             # ライセンスファイル
├── README.md                           # プロジェクト説明
└── Qx.sln                             # ソリューションファイル
```

## 📝 命名規則

### ファイル命名規則

| 種別 | 命名規則 | 例 |
|------|----------|-----|
| C#クラス | PascalCase.cs | QueryCommand.cs |
| インターフェース | IPascalCase.cs | IOpenAIService.cs |
| テストクラス | ClassNameTests.cs | QueryCommandTests.cs |
| 拡張メソッド | TypeExtensions.cs | StringExtensions.cs |
| 設定ファイル | lowercase.ext | nuget.config |
| Markdownドキュメント | UPPERCASE.md | README.md |
| YAMLファイル | kebab-case.yml | ci.yml |
| シェルスクリプト | lowercase.sh | build.sh |

### ディレクトリ命名規則

| 種別 | 命名規則 | 例 |
|------|----------|-----|
| ソースコード | PascalCase（単数形） | Command/, Service/ |
| テスト | PascalCase（単数形） | Command/, Service/ |
| ドキュメント | lowercase | docs/ |
| スクリプト | lowercase（単数形） | script/ |
| 設定 | .lowercase | .github/ |
| 出力 | lowercase | bin/, obj/, publish/ |

### 名前空間規則

```csharp
// ルート名前空間
namespace Qx;

// サブ名前空間（ディレクトリに対応）
namespace Qx.Command;
namespace Qx.Service;
namespace Qx.Model;
namespace Qx.Handler;
namespace Qx.Formatter;
namespace Qx.Validator;
namespace Qx.Extension;
namespace Qx.Exception;

// テスト名前空間
namespace Qx.Tests.Command;
namespace Qx.Tests.Service;
namespace Qx.IntegrationTests;
```

## 🎯 主要ファイル説明

### エントリーポイント

#### Program.cs
- **役割**: アプリケーションのエントリーポイント
- **責務**: 
  - DIコンテナの構築
  - コマンドライン解析の初期化
  - アプリケーションの起動

### プロジェクト設定

#### Qx.csproj
- **役割**: プロジェクト定義ファイル
- **内容**:
  - ターゲットフレームワーク: net9.0
  - AoT設定: PublishAot=true
  - パッケージ参照定義

#### Directory.Build.props
- **役割**: 共通ビルド設定
- **内容**:
  - 共通のプロパティ
  - コード分析ルール
  - ビルド最適化設定

#### Directory.Packages.props
- **役割**: 集中パッケージバージョン管理
- **内容**:
  - NuGetパッケージバージョン統一
  - Central Package Management設定

### 設定ファイル

#### global.json
- **役割**: .NET SDKバージョン固定
- **内容**:
  - SDK バージョン指定
  - ロールフォワード設定

#### nuget.config
- **役割**: NuGet設定
- **内容**:
  - パッケージソース定義
  - 復元設定

#### .editorconfig
- **役割**: コードスタイル設定
- **内容**:
  - インデント設定
  - 改行コード
  - C#コーディング規約

## 🔗 モジュール構成

### 依存関係の方向

```
Program.cs
    ↓
Command ← Handler ← Service
    ↓        ↓         ↓
Model    Validator  Formatter
    ↓        ↓         ↓
Extension ← Exception
```

### レイヤー境界

```
Presentation Layer
├── Command/
├── Formatter/
└── Program.cs

Application Layer
├── Handler/
├── Service/
└── Validator/

Domain Layer
├── Model/
└── Exception/

Cross-Cutting
└── Extension/
```

### パッケージ依存関係

```
Qx (Main Project)
├── OpenAI (2.3.0)
├── System.CommandLine (2.0.0-beta6)
└── Microsoft.Extensions.*

Qx.Tests
├── xUnit (2.9.3)
├── FluentAssertions (7.0.0)
├── Moq (4.20.72)
└── → Qx (Project Reference)

Qx.IntegrationTests
├── xUnit (2.9.3)
├── Microsoft.AspNetCore.TestHost
└── → Qx (Project Reference)
```

## 📂 出力ディレクトリ

### ビルド出力

```
bin/
├── Debug/
│   └── net9.0/
│       ├── Qx.dll
│       ├── Qx.pdb
│       └── *.deps.json
└── Release/
    └── net9.0/
        └── publish/
            └── Qx (実行可能ファイル)

obj/
├── Debug/
│   └── net9.0/
└── Release/
    └── net9.0/
```

### パブリッシュ出力

```
publish/
├── linux-x64/
│   └── qx
├── win-x64/
│   └── qx.exe
└── osx-universal/
    └── qx
```

## 🚫 除外ファイル（.gitignore）

```
# ビルド出力
bin/
obj/
publish/

# ユーザー固有
*.user
*.suo
.vs/

# IDE
.idea/
*.swp
.DS_Store

# テスト結果
TestResults/
*.coverage
*.trx

# ログ
*.log
```

## 📌 ディレクトリ作成規則

### 新規機能追加時

1. **新しいコマンド追加**
   - src/Qx/Command/に配置
   - 対応するテストをtest/Qx.Tests/Command/に作成

2. **新しいサービス追加**
   - src/Qx/Service/に配置
   - インターフェースも同ディレクトリ

3. **新しいモデル追加**
   - src/Qx/Model/に配置
   - 値オブジェクトも同ディレクトリ

### ファイル配置の原則

- 1ファイル1クラス
- インターフェースと実装は同じディレクトリ
- 関連するクラスは同じディレクトリにグループ化
- テストは本体と同じ構造をミラーリング

## 🔄 変更履歴

| 日付 | バージョン | 変更内容 | 変更者 |
|------|------------|----------|--------|
| 2025-08-12 | 1.0.0 | 初版作成 | Development Team |

---

*このドキュメントはプロジェクトの物理構造を示すものです。論理設計は [DESIGN.md](./DESIGN.md) を参照してください。*