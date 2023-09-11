import { Photo } from "./photo"

export interface Member {
    id: number
    userName: string
    urlPhoto: string
    age: number
    knownAs: string
    cretaed: string
    lastActive: string
    gender: string
    introduction: string
    lookingFor: string
    interests: string
    city: string
    country: string
    photos: Photo[]
  }