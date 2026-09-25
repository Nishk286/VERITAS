from pathlib import Path

import joblib

from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.linear_model import LogisticRegression
from sklearn.pipeline import Pipeline

from training_data import TRAINING_TEXTS, TRAINING_LABELS


MODEL_PATH = Path(__file__).parent / "veritas_model.joblib"


def train_model():
    model = Pipeline(
        [
            (
                "tfidf",
                TfidfVectorizer(
                    lowercase=True,
                    stop_words="english",
                    ngram_range=(1, 2)
                )
            ),
            (
                "classifier",
                LogisticRegression(
                    max_iter=1000
                )
            )
        ]
    )

    model.fit(
        TRAINING_TEXTS,
        TRAINING_LABELS
    )

    joblib.dump(
        model,
        MODEL_PATH
    )

    return model


def load_model():
    if not MODEL_PATH.exists():
        return train_model()

    return joblib.load(MODEL_PATH)



if __name__ == "__main__":
    model = train_model()

    print("VERITAS ML model trained successfully.")
    print(f"Model saved to: {MODEL_PATH}")