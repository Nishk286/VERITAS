from pathlib import Path

import joblib
from fastapi import FastAPI
from pydantic import BaseModel


MODEL_PATH = Path(__file__).parent / "veritas_model.joblib"

model = joblib.load(MODEL_PATH)

app = FastAPI(
    title="VERITAS ML Service",
    description="Machine-learning classification service for VERITAS.",
    version="1.0.0"
)


class ClaimRequest(BaseModel):
    claim: str


class ClassificationResponse(BaseModel):
    label: str
    confidence: float
    model_name: str
    available: bool


@app.get("/")
def root():
    return {
        "service": "VERITAS ML Service",
        "status": "running",
        "model": "veritas_model.joblib"
    }


@app.post("/classify", response_model=ClassificationResponse)
def classify(request: ClaimRequest):

    claim = request.claim.strip()

    if not claim:
        return ClassificationResponse(
            label="UNCERTAIN",
            confidence=0,
            model_name="VERITAS TF-IDF + Logistic Regression",
            available=False
        )

    probabilities = model.predict_proba([claim])[0]

    predicted_index = probabilities.argmax()

    label = model.classes_[predicted_index]

    confidence = float(
        probabilities[predicted_index] * 100
    )

    return ClassificationResponse(
        label=label,
        confidence=round(confidence, 2),
        model_name="VERITAS TF-IDF + Logistic Regression",
        available=True
    )