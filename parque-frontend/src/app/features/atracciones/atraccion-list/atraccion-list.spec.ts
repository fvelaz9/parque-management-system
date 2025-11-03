import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AtraccionListComponent } from './atraccion-list';

describe('AtraccionList', () => {
  let component: AtraccionListComponent;
  let fixture: ComponentFixture<AtraccionListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AtraccionListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AtraccionListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
