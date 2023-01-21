import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { IPhoneNumber } from '../models/iphone-number';
import { IPhoneNumberType } from '../models/iphone-number-type';
import { ITag } from '../models/itag';

@Component({
  selector: 'app-add-card',
  templateUrl: './add-card.component.html',
  styleUrls: ['./add-card.component.css', '../app.component.css', '../bootstrap.min.css']
})


export class AddCardComponent {

  UserId: number = 0;
  Waiting: boolean = false;
  PhoneNumberTypes: IPhoneNumberType[] = [];
  StateCodes: [] = [];
  Tags: ITag[] = [];
  Errors: [] = [];
  PhoneNumbers: IPhoneNumber[] = [];
  NewPhoneNumber: string = '';
  NewPhoneNumberType: IPhoneNumberType = {
      Name: 'Work',
      PhoneNumberTypeId: 0,
      Deleted: false
  };
  Address1: string = '';
  Address2: string = '';
  City: string = '';
  StateCode: string = '';
  Zip: string = '';
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
  NewTag: string = '';
  NewCardSaved: boolean = false;

  constructor() {
    this.UserId = 0;
    this.Waiting = false;
    this.StateCodes = [];
    this.Tags = [];
    this.Errors = [];
    this.PhoneNumbers = [];
    this.PhoneNumberTypes = [];
    this.TagTypes = [];
    this.Address1 = '';
    this.Address2 = '';
    this.City = '';
    this.StateCode = '';
    this.Zip = '';
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
    this.NewTag = '';
    this.NewCardSaved = false;

    this.AddPhoneNumber('');
  }

  RemovePhoneNumber(pn: string) {
    for (let i = 0; i < this.PhoneNumbers.length; i++) {
      if (this.PhoneNumbers[i].Number == pn) {
        this.PhoneNumbers.splice(i, 1);
      }
    }
  };

  AddPhoneNumber(pn: string) {
    let aNumbers: IPhoneNumber[] = [];
    aNumbers.push(
      {
        PhoneNumberId: 0,
        PhoneNumberTypeId: 0,
        Number: pn,
        Extension: '',
        Deleted: false
      }
    );
    this.PhoneNumbers = aNumbers;
  };

  ResetBackImage() { };

  RemoveTag(t: string) {
    for (let i = 0; i < this.Tags.length; i++) {
      if (this.Tags[i].Text == t) {
        this.Tags.splice(i, 1);
      }
    }
  };

  AddTag() {
    let aTags: ITag[] = [];
    for (let i = 0; i < this.Tags.length; i++) {
      aTags.push(this.Tags[i]);
    }
    
    aTags.push({ Text: this.NewTag, TagType: 2 });

    this.NewTag = '';
    this.Tags = aTags;
  };

  public trackItem(index: number, item: IPhoneNumber) {
    return item.PhoneNumberId;
  }

}
