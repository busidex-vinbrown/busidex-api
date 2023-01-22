import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { IPhoneNumber } from '../models/iphone-number';
import { IPhoneNumberType } from '../models/iphone-number-type';
import { ITag } from '../models/itag';
import { DomSanitizer } from '@angular/platform-browser';
import { ICardAddress } from '../models/icard-address';
import { IState } from '../models/iState';

@Component({
  selector: 'app-add-card',
  templateUrl: './add-card.component.html',
  styleUrls: ['../app.component.css', '../bootstrap.min.css', './add-card.component.css']
})


export class AddCardComponent {

  UserId: number = 0;
  Waiting: boolean = false;
  PhoneNumberTypes: IPhoneNumberType[] = [];
  StateCodes: IState[] = [];
  Tags: ITag[] = [];
  Errors: [] = [];
  PhoneNumbers: IPhoneNumber[] = [];
  NewPhoneNumber: string = '';
  NewPhoneNumberType: IPhoneNumberType[][] = [];
  Address: ICardAddress;
  TagTypes: [] = [];
  FrontFileId: string = '';
  BackFileId: string = '';
  CardId: number = 0;
  IsValid: boolean = false;
  CompanyName: string = '';
  Name: string = '';
  Title: string = '';
  Email: string = '';
  Url: string = '';
  Visibility: number = 0;
  FrontImage: string = '';
  FrontOrientation: string = '';
  BackImage: string = '';
  BackOrientation: string = '';
  IsMyCard: boolean = false;
  NewCardSaved: boolean = false;

  _sanitizer: DomSanitizer;

  constructor(sanitizer: DomSanitizer) {
    this._sanitizer = sanitizer;

    this.addStateCodes();

    this.UserId = 0;
    this.Waiting = false;
    this.Tags = [];
    this.Errors = [];
    this.PhoneNumbers = [];
    this.PhoneNumberTypes = [];
    this.TagTypes = [];
    this.Address = {
      cardAddressId: -1,
      cardId: -1,
      address1: '',
      address2: '',
      city: '',
      stateCode: this.StateCodes[0],
      stateCodeId: -1,
      zipCode: '',
      region: '',
      country: '',
      longitude: -1,
      latitude: -1,
      deleted: false
    };
    this.FrontFileId = '';
    this.BackFileId = '';
    this.CardId = 0;
    this.IsValid = false;
    this.CompanyName = '';
    this.Name = '';
    this.Title = '';
    this.Email = '';
    this.Url = '';
    this.Visibility = 1;
    this.FrontImage = '';
    this.FrontOrientation = '';
    this.BackImage = '';
    this.BackOrientation = '';
    this.IsMyCard = false;
    this.NewCardSaved = false;

    
    for (let i = 0; i < 3; i++) {
      this.NewPhoneNumberType.push(
        [
          {
            Name: 'Work',
            PhoneNumberTypeId: 0,
            Deleted: false
          },
          {
            Name: 'Mobile',
            PhoneNumberTypeId: 1,
            Deleted: false
          },
          {
            Name: 'Home',
            PhoneNumberTypeId: 2,
            Deleted: false
          },
          {
            Name: 'Fax',
            PhoneNumberTypeId: 1,
            Deleted: false
          },
          {
            Name: 'Other',
            PhoneNumberTypeId: 1,
            Deleted: false
          }
        ]
      );
      this.PhoneNumbers.push(
        {
          PhoneNumberId: 0,
          PhoneNumberTypeId: -1,
          Number: '',
          Extension: '',
          Deleted: false
        }
      );
    }

    for (let i = 0; i < 7; i++) {
      this.addTag({
        TagId: 0,
        Text: '',
        TagTypeId: 2
      });
    }

    
  }

  photoURL(img: string) {
    let path = this._sanitizer.bypassSecurityTrustUrl(img);
    console.log(img);
    console.log(path);
    return path;
  }

  addTag(t: ITag) {
    this.Tags.push(t);
  };

  addStateCodes() {
    this.StateCodes = [];
    this.StateCodes.push({ stateCodeId: -1, code: '', name: '' });
    this.StateCodes.push({ stateCodeId: 1, code: 'AL', name: 'Alabama' });
    this.StateCodes.push({ stateCodeId: 2, code: 'AK', name: 'Alaska' });
    this.StateCodes.push({ stateCodeId: 3, code: 'AZ', name: 'Arizona' });
    this.StateCodes.push({ stateCodeId: 4, code: 'AR', name: 'Arkansas' });
    this.StateCodes.push({ stateCodeId: 5, code: 'CA', name: 'California' });
    this.StateCodes.push({ stateCodeId: 6, code: 'CO', name: 'Colorado' });
    this.StateCodes.push({ stateCodeId: 7, code: 'CT', name: 'Connecticut' });
    this.StateCodes.push({ stateCodeId: 8, code: 'DE', name: 'Delaware' });
    this.StateCodes.push({ stateCodeId: 9, code: 'DC', name: 'District Of Columbia' });
    this.StateCodes.push({ stateCodeId: 10, code: 'FL', name: 'Florida' });
    this.StateCodes.push({ stateCodeId: 11, code: 'GA', name: 'Georgia' });
    this.StateCodes.push({ stateCodeId: 12, code: 'HI', name: 'Hawaii' });
    this.StateCodes.push({ stateCodeId: 13, code: 'ID', name: 'Idaho' });
    this.StateCodes.push({ stateCodeId: 14, code: 'IL', name: 'Illinois' });
    this.StateCodes.push({ stateCodeId: 15, code: 'IN', name: 'Indiana' });
    this.StateCodes.push({ stateCodeId: 16, code: 'IA', name: 'Iowa' });
    this.StateCodes.push({ stateCodeId: 17, code: 'KS', name: 'Kansas' });
    this.StateCodes.push({ stateCodeId: 18, code: 'KY', name: 'Kentucky' });
    this.StateCodes.push({ stateCodeId: 19, code: 'LA', name: 'Louisiana' });
    this.StateCodes.push({ stateCodeId: 20, code: 'ME', name: 'Maine' });
    this.StateCodes.push({ stateCodeId: 21, code: 'MD', name: 'Maryland' });
    this.StateCodes.push({ stateCodeId: 22, code: 'MA', name: 'Massachusetts' });
    this.StateCodes.push({ stateCodeId: 23, code: 'MI', name: 'Michigan' });
    this.StateCodes.push({ stateCodeId: 24, code: 'MN', name: 'Minnesota' });
    this.StateCodes.push({ stateCodeId: 25, code: 'MS', name: 'Mississippi' });
    this.StateCodes.push({ stateCodeId: 26, code: 'MO', name: 'Missouri' });
    this.StateCodes.push({ stateCodeId: 27, code: 'MT', name: 'Montana' });
    this.StateCodes.push({ stateCodeId: 28, code: 'NE', name: 'Nebraska' });
    this.StateCodes.push({ stateCodeId: 29, code: 'NV', name: 'Nevada' });
    this.StateCodes.push({ stateCodeId: 30, code: 'NH', name: 'New Hampshire' });
    this.StateCodes.push({ stateCodeId: 31, code: 'NJ', name: 'New Jersey' });
    this.StateCodes.push({ stateCodeId: 32, code: 'NM', name: 'New Mexico' });
    this.StateCodes.push({ stateCodeId: 33, code: 'NY', name: 'New York' });
    this.StateCodes.push({ stateCodeId: 34, code: 'NC', name: 'North Carolina' });
    this.StateCodes.push({ stateCodeId: 35, code: 'ND', name: 'North Dakota' });
    this.StateCodes.push({ stateCodeId: 36, code: 'OH', name: 'Ohio' });
    this.StateCodes.push({ stateCodeId: 37, code: 'OK', name: 'Oklahoma' });
    this.StateCodes.push({ stateCodeId: 38, code: 'OR', name: 'Oregon' });
    this.StateCodes.push({ stateCodeId: 39, code: 'PA', name: 'Pennsylvania' });
    this.StateCodes.push({ stateCodeId: 40, code: 'RI', name: 'Rhode Island' });
    this.StateCodes.push({ stateCodeId: 41, code: 'SC', name: 'South Carolina' });
    this.StateCodes.push({ stateCodeId: 42, code: 'SD', name: 'South Dakota' });
    this.StateCodes.push({ stateCodeId: 43, code: 'TN', name: 'Tennessee' });
    this.StateCodes.push({ stateCodeId: 44, code: 'TX', name: 'Texas' });
    this.StateCodes.push({ stateCodeId: 45, code: 'UT', name: 'Utah' });
    this.StateCodes.push({ stateCodeId: 46, code: 'VT', name: 'Vermont' });
    this.StateCodes.push({ stateCodeId: 47, code: 'VA', name: 'Virginia' });
    this.StateCodes.push({ stateCodeId: 48, code: 'WA', name: 'Washington' });
    this.StateCodes.push({ stateCodeId: 49, code: 'WV', name: 'West Virginia' });
    this.StateCodes.push({ stateCodeId: 50, code: 'WI', name: 'Wisconsin' });
    this.StateCodes.push({ stateCodeId: 51, code: 'WY', name: 'Wyoming' });
  }

  saveCard() {
    let jaddress = JSON.stringify(this.Address);
    console.log(`Address: ${jaddress}`);
  }
}
