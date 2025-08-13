# Qx ドキュメント目次

---
version: 1.1.0
last_updated: 2025-08-13
author: Arstella Ltd.
status: approved
---

## 📚 ドキュメント一覧

Qx (Query eXpress) プロジェクトのドキュメントへの索引です。

### 📋 仕様・設計

| ドキュメント | 説明 | 対象読者 | 更新頻度 |
|-------------|------|----------|----------|
| [01_PRODUCT.md](./01_PRODUCT.md) | 製品概要とビジョン | プロダクトマネージャー、開発者 | 四半期 |
| [02_REQUIREMENTS.md](./02_REQUIREMENTS.md) | 機能要求定義 | 開発者、QAエンジニア | 機能追加時 |
| [03_TECH.md](./03_TECH.md) | 技術仕様と実装詳細 | 開発者 | 実装変更時 |
| [04_DESIGN.md](./04_DESIGN.md) | アーキテクチャ設計 | アーキテクト、開発者 | 構造変更時 |
| [05_STRUCTURE.md](./05_STRUCTURE.md) | プロジェクト構造 | 新規開発者 | 構造変更時 |
| [06_TASK.md](./06_TASK.md) | タスク管理とロードマップ | 全チームメンバー | 毎週 |
| [07_CLI_SPEC.md](./07_CLI_SPEC.md) | **CLIコマンド仕様** | 開発者、ユーザー | 機能変更時 |

### 📖 利用ガイド

| ドキュメント | 説明 | 対象読者 |
|-------------|------|----------|
| [08_EXAMPLE.md](./08_EXAMPLE.md) | 使用例集 | エンドユーザー、開発者 |
| [../README.md](../README.md) | プロジェクト概要とクイックスタート | 新規ユーザー |

## 🔍 クイックリンク

### 開発を始める方
1. [README.md](../README.md) - インストールと基本的な使い方
2. [03_TECH.md](./03_TECH.md) - 開発環境のセットアップ
3. [05_STRUCTURE.md](./05_STRUCTURE.md) - コードベースの理解

### CLIの仕様を確認する方
1. [07_CLI_SPEC.md](./07_CLI_SPEC.md) - コマンド仕様の詳細
2. [08_EXAMPLE.md](./08_EXAMPLE.md) - 実用的な使用例

### プロジェクトに貢献する方
1. [06_TASK.md](./06_TASK.md) - 現在のタスクと優先順位
2. [02_REQUIREMENTS.md](./02_REQUIREMENTS.md) - 実装すべき機能
3. [04_DESIGN.md](./04_DESIGN.md) - アーキテクチャの理解

## 📝 ドキュメント管理方針

### 更新ルール
- **即座に更新**: 06_TASK.md、07_CLI_SPEC.md（コマンド変更時）
- **実装と同時**: 03_TECH.md、05_STRUCTURE.md
- **リリース前**: README.md、08_EXAMPLE.md
- **定期更新**: 01_PRODUCT.md（四半期）、04_DESIGN.md（大規模変更時）

### Single Source of Truth
- CLI仕様: 07_CLI_SPEC.md
- API仕様: 03_TECH.md内のOpenAI統合セクション
- プロジェクト構造: 05_STRUCTURE.md
- タスク状況: 06_TASK.md

### ドキュメント品質基準
- ✅ 対象読者が明確
- ✅ バージョン情報とステータスを記載
- ✅ 他のドキュメントと重複なし
- ✅ コード例が動作確認済み
- ✅ 相互参照が正確

## 🏷️ タグ索引

### 技術タグ
- `#openai`: OpenAI API関連 → [03_TECH.md](./03_TECH.md)
- `#cli`: CLIインターフェース → [07_CLI_SPEC.md](./07_CLI_SPEC.md)
- `#dotnet`: .NET/C#実装 → [03_TECH.md](./03_TECH.md)
- `#websearch`: Web検索機能 → [02_REQUIREMENTS.md](./02_REQUIREMENTS.md)
- `#functions`: 関数呼び出し機能 → [07_CLI_SPEC.md](./07_CLI_SPEC.md)
- `#license`: ライセンス表示機能 → [07_CLI_SPEC.md](./07_CLI_SPEC.md)

### ユーザータグ
- `#quickstart`: クイックスタート → [README.md](../README.md)
- `#examples`: 使用例 → [08_EXAMPLE.md](./08_EXAMPLE.md)
- `#troubleshooting`: トラブルシューティング → [03_TECH.md](./03_TECH.md)

## 🔄 変更履歴

| 日付 | バージョン | 変更内容 | 変更者 |
|------|------------|----------|--------|
| 2025-08-13 | 1.1.0 | ファイル名を番号プレフィックス形式に変更、ライセンス機能追加 | Arstella Ltd. |
| 2025-08-13 | 1.0.0 | 初版作成、CLI_SPEC.md追加 | Development Team |

---

*このインデックスは新しいドキュメントが追加されるたびに更新してください。*