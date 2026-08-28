# ♠️ Poker Unity Game

[![Unity](https://img.shields.io/badge/Unity-6000-black.svg)](https://unity.com)
[![C#](https://img.shields.io/badge/C%23-.NET%20Standard%202.1-purple.svg)](https://learn.microsoft.com/en-us/dotnet/standard/net-standard)
[![Platform](https://img.shields.io/badge/Platform-Windows-blue.svg)](https://www.microsoft.com/windows)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

> Карточный покер на Unity, где вы сражаетесь один на один с ИИ-ботом. Реализованы все основные комбинации и упрощённая логика ставок.

![Геймплей]

![[Pasted image 20260828131038.png]]

---

## 🎮 О проекте

**Poker Unity Game** — это учебный проект, созданный для демонстрации навыков разработки на Unity и C#. Игра представляет собой классический Техасский Холдем против ИИ-противника с реалистичной логикой комбинаций и адаптивным поведением бота.

Проект полностью самостоятельный — от архитектуры до реализации ИИ.

---

## 🛠️ Технологический стек

| Компонент | Технология |
|-----------|------------|
| **Движок** | Unity 6000 |
| **Язык** | C# (.NET Standard 2.1) |
| **Платформа** | ПК (Windows) |
| **Архитектура** | ООП с разделением на GameManager, Deck, Card, Player, Bot, HandEvaluator |

---

## 🧠 Основные механики

| Механика | Описание |
|----------|----------|
| 🃏 **Колода** | 52 карты, случайное перемешивание (алгоритм Фишера-Йетса) |
| 🤝 **Раздача** | По 2 карманные карты игроку и боту, 5 общих карт на столе |
| 🏆 **Комбинации** | От старшей карты до флеш-рояля — все 10 комбинаций |
| 🤖 **ИИ бота** | Адаптивное поведение: чек / колл / рейз на основе силы карт |
| 💰 **Банк** | Отображение текущего банка и результата раунда |

---

## 🎬 Скриншоты

| Начало игры | Раздача и ставки | Открытие трёх карт |
|:---:|:---:|:---:|
| ![Начало](![[Pasted image 20260828130943.png]]) | ![Раздача](![[Pasted image 20260828130953.png]]) | ![Флоп](![[Pasted image 20260828131038.png]]) |

| Победа после сброса врага | Враг идёт ва-банк | Конец игры |
|:---:|:---:|:---:|
| ![Победа](![[Pasted image 20260828131123.png]]) | ![Ва-банк](![[Pasted image 20260828131246.png]]) | ![Конец](![[Pasted image 20260828131313.png]]) |

---

## 🧩 Архитектура проекта

```mermaid
classDiagram
    class GameManager {
        -Deck deck
        -Player player
        -Bot bot
        -int pot
        +StartRound()
        +PlaceBet()
        +Showdown()
    }
    class Deck {
        -List~Card~ cards
        +Shuffle()
        +DealCard()
    }
    class Card {
        -Suit suit
        -Rank rank
        +ToString()
    }
    class HandEvaluator {
        +EvaluateHand(List~Card~ hand)
        +CompareHands()
    }
    class Player {
        -List~Card~ hand
        -int chips
        +Fold()
        +Call()
        +Raise()
    }
    class Bot {
        -List~Card~ hand
        -int chips
        +MakeDecision()
        +CalculateHandStrength()
    }

    GameManager --> Deck
    GameManager --> Player
    GameManager --> Bot
    GameManager --> HandEvaluator
    Player --> Card
    Bot --> Card
    HandEvaluator --> Card
