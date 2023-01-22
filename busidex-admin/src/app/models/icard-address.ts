import { IState } from "./iState";

export interface ICardAddress {
  cardAddressId: number;
  cardId: number;
  address1: string;
  address2: string;
  city: string;
  stateCode: IState | undefined;
  stateCodeId: number;
  zipCode: string;
  region: string;
  country: string;
  deleted: boolean;
  latitude: number;
  longitude: number;
}
